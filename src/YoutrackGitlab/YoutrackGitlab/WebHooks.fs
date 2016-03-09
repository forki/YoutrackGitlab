namespace YoutrackGitlab

[<AutoOpen>]
module WebHooks =

    #if INTERACTIVE
    System.IO.Directory.SetCurrentDirectory(__SOURCE_DIRECTORY__)
    #endif

    open System.Text.RegularExpressions

    open FSharp.Data


    type CommentCommitEvent = JsonProvider<"CommentCommit.json", EmbeddedResource="YoutrackGitlab, CommentCommit.json">

    type CommitCommentCommand = { TicketId: string
                                  Comment: string
                                  CommitUrl: string }

    let financeRegex () =
        let regex = new Regex("""(BF-(\d+))""")
        regex

    let extractTicketNr message =
        let regex = financeRegex()
        let matching = regex.Match(message)
        match matching.Success with
        | true  -> matching.Groups.[0].Value
        | false -> ""

    let eventToCommand (event:CommentCommitEvent.Root) =
        let ticketNr = extractTicketNr event.Commit.Message
        { TicketId = ticketNr
          Comment = event.Commit.Message
          CommitUrl = event.Commit.Url }

