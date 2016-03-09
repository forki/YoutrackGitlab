
open YouTrackSharp.Infrastructure
open YouTrackSharp.Projects

type Credentials = { Username : string
                     Password : string }

let credentials =
    let username = System.Configuration.ConfigurationManager.AppSettings.["Username"]
    let password = System.Configuration.ConfigurationManager.AppSettings.["Password"]
    { Username = username
      Password = password }

[<EntryPoint>]
let main argv =
    let connection = new Connection("youtrack.rhein-spree.com",8443, true)
    connection.Authenticate(credentials.Username, credentials.Password)

    let pm = new ProjectManagement(connection)
    let projects = pm.GetProjects()

    for project in projects do
        printfn "%A" project.ShortName

    printfn "%A" (pm.GetProject("BI").Name)

    0 // return an integer exit code
