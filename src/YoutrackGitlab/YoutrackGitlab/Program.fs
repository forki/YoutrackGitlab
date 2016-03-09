
open Suave
open Suave.Filters
open Suave.Operators
open Suave.Successful

open YouTrackSharp.Infrastructure
open YouTrackSharp.Projects

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

[<EntryPoint>]
let main argv =
    let connection = new Connection(settings.Host,settings.Port, settings.UseSsl, settings.Path)
    connection.Authenticate(settings.Username, settings.Password)

    let pm = new ProjectManagement(connection)
    let projects = pm.GetProjects()

    for project in projects do
        printfn "%A" project.ShortName

    printfn "%A" (pm.GetProject("BI").Name)

    startWebServer defaultConfig (POST >=> (path "/comment" >=> OK "Comment"))

    0 // return an integer exit code
