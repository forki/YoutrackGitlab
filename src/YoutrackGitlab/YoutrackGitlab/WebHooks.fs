module YoutrackGitlab.WebHooks

open FSharp.Data

type Comment = JsonProvider<"GitlabWebHooks/Comment.json">