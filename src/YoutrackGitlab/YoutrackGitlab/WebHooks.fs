module YoutrackGitlab.WebHooks

open FSharp.Data

type CommentEvent = JsonProvider<"GitlabWebHooks/Comment.json">