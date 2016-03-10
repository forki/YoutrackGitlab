
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open YouTrackSharp.Infrastructure
open YouTrackSharp.Issues

open YoutrackGitlab.WebHooks

type YoutrackSettings = { Username : string
                          Password : string
                          Host: string
                          Port: int
                          Path: string
                          UseSsl: bool }

let settings =
    let username = System.Configuration.ConfigurationManager.AppSettings.["Username"]
    let password = System.Configuration.ConfigurationManager.AppSettings.["Password"]
    let host = System.Configuration.ConfigurationManager.AppSettings.["Host"]
    let port = int System.Configuration.ConfigurationManager.AppSettings.["Port"]
    let path = System.Configuration.ConfigurationManager.AppSettings.["Path"]
    let useSsl = match System.Configuration.ConfigurationManager.AppSettings.["UseSsl"] with
                 | "True" -> true
                 | "true" -> true
                 | _ -> false

    { Username = username
      Password = password
      Host = host
      Port = port
      Path = path
      UseSsl = useSsl }

let issueManagement =
    let connection = new Connection(settings.Host,settings.Port, settings.UseSsl, settings.Path)
    connection.Authenticate(settings.Username, settings.Password)
    new IssueManagement(connection)

let toYoutrackComment command =
    let beginning = "New comment on commit in gitlab:\n\n"
    let quotedComment = ">" + command.Comment.Replace("\n","\n>")
    sprintf "%s%s\n\n%s" beginning quotedComment command.CommitUrl

let processAsync ctx =
    async{
        let json = System.Text.Encoding.UTF8.GetString ctx.request.rawForm
        let result = eventToCommand (jsonToCommentCommitEvent json)
        let comment = toYoutrackComment result
        issueManagement.ApplyCommand(result.TicketId, "", comment)
        return! (OK "Comment" ctx)
    }

[<EntryPoint>]
let main argv =
    startWebServer defaultConfig (POST >=> (path "/comment" >=> warbler (fun c -> processAsync)))

    0 // return an integer exit code
