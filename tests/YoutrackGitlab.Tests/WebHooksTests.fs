namespace YoutrackGitlab.Tests.WebHooksTests

open NUnit.Framework
open FsUnit
open Swensen.Unquote

open YoutrackGitlab.WebHooks

module ``When extracting comment commit command of comment commit event`` =
    let sample = CommentCommitEvent.GetSample() //Placed CommentCommit.json into this assembly aswell because EmbeddedResource seems not working
    let command = eventToCommand sample

    [<Test>]
    let ``It should have extracted the correct ticket id`` () =
        test <@ command.TicketId = "BF-314" @>

    [<Test>]
    let ``It should have extracted the correct comment`` () =
        test <@ command.Comment = "This is a commit comment. How does this work?" @>

    [<Test>]
    let ``It should have extracted the correct user name`` () =
        test <@ command.User = "root" @>