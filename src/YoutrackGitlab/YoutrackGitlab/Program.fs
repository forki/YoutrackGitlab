
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open YouTrackSharp.Infrastructure
open YouTrackSharp.Admin
open YouTrackSharp.Issues

open YoutrackGitlab.WebHooks

type YoutrackSettings = { Username : string
                          Password : string
                          Host: string
                          Port: int
                          Path: string
                          UseSsl: bool }

type ServiceSettings = { Host: string
                         Port: int}


let (|StringIsNotNullOrWhitespace|_|) str =
    match (not (System.String.IsNullOrWhiteSpace(str))) with
    | true -> Some(str)
    | _ -> None

let serviceSettings =
    let host = match System.Configuration.ConfigurationManager.AppSettings.["Host"] with
               | StringIsNotNullOrWhitespace str -> str
               | _ -> "127.0.0.1"

    let port = match System.Configuration.ConfigurationManager.AppSettings.["Port"] with
               | StringIsNotNullOrWhitespace str -> int str
               | _ -> 8083
    { Host = host
      Port = port }

let youtrackSettings =
    let username = System.Configuration.ConfigurationManager.AppSettings.["Youtrack.Username"]
    let password = System.Configuration.ConfigurationManager.AppSettings.["Youtrack.Password"]
    let host = System.Configuration.ConfigurationManager.AppSettings.["Youtrack.Host"]
    let port = int System.Configuration.ConfigurationManager.AppSettings.["Youtrack.Port"]
    let path = System.Configuration.ConfigurationManager.AppSettings.["Youtrack.Path"]
    let useSsl = match System.Configuration.ConfigurationManager.AppSettings.["Youtrack.UseSsl"] with
                 | "True" -> true
                 | "true" -> true
                 | _ -> false

    { Username = username
      Password = password
      Host = host
      Port = port
      Path = path
      UseSsl = useSsl }

let connection =
    let connection = new Connection(youtrackSettings.Host,youtrackSettings.Port, youtrackSettings.UseSsl, youtrackSettings.Path)
    connection.Authenticate(youtrackSettings.Username, youtrackSettings.Password)
    connection

let youtrackUsers =
    let usermanagement = new UserManagement(connection)
    Seq.map (fun (x:User) -> x.Username) (usermanagement.GetAllUsers())

let (|IsYoutrackUser|_|) user =
    match Seq.contains user youtrackUsers with
    | true -> Some(user)
    | _ -> None

let issueManagement = new IssueManagement(connection)

let toYoutrackComment command =
    let beginning = sprintf "New comment on commit in gitlab from %s:\n\n" command.User
    let quotedComment = ">" + command.Comment.Replace("\n","\n>")
    sprintf "%s%s\n\n%s" beginning quotedComment command.CommitUrl

let processAsync ctx =
    async{
        let json = System.Text.Encoding.UTF8.GetString ctx.request.rawForm
        let result = eventToCommand (jsonToCommentCommitEvent json)
        let comment = toYoutrackComment result
        let runas = match result.User with
                    | IsYoutrackUser user -> user
                    | _ -> null
        issueManagement.ApplyCommand(result.TicketId, "", comment, false, runas)
        return! (OK "Comment" ctx)
    }

[<EntryPoint>]
let main argv =
    let config = defaultConfig.withBindings [HttpBinding.mkSimple HTTP serviceSettings.Host serviceSettings.Port]
    startWebServer config (POST >=> (path "/comment" >=> warbler (fun c -> processAsync)))

    0 // return an integer exit code
