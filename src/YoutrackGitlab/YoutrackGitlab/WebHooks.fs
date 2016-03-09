namespace YoutrackGitlab

[<AutoOpen>]
module WebHooks =
    open System.Text.RegularExpressions
    open FSharp.Data

    type CommentCommitEvent = JsonProvider<"CommentCommit.json", EmbeddedResource="YoutrackGitlab, CommentCommit.json">
    type CommitCommentCommand = { TicketId: string
                                  Comment: string
                                  CommitUrl: string }

    let financeRegex () =
        let regex = new Regex("""(^|\s)([A-Z]+-(\d+))""")
        regex

    let extractTicketNr message =
        let regex = financeRegex()
        let matching = regex.Match(message)
        match matching.Success with
        | true  -> matching.Groups.[2].Value
        | false -> ""

    let eventToCommand (event:CommentCommitEvent.Root) =
        let ticketNr = extractTicketNr event.Commit.Message
        { TicketId = ticketNr
          Comment = event.Commit.Message
          CommitUrl = event.Commit.Url }

