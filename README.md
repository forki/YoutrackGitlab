# YoutrackGitlab
A bridge which posts comments created on commits in Gitlab to Youtrack using webhooks

Powered by Fsharp.Data and Suave.io

The endpoint ``http://<yourhost>/comment`` has to be configured as webhook for comments in Gitlab. On every new comment the bridge tries 
to extract the corresping Youtrack-Ticket-Id out of the commented commit. It then creates a comment on the youtrack ticket with a link to
corresponding Gitlab-Commit.
