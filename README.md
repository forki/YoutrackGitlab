# YoutrackGitlab
A bridge which posts comments created on commits in Gitlab to Youtrack using webhooks

Powered by Fsharp.Data and Suave.io

The endpoint ``http://<yourhost>/comment`` has to be configured as webhook for comments in Gitlab. On every new comment on commits the bridge tries 
to extract the corresping Youtrack-Ticket-Id out of the message of the commented commit. It then creates a comment on this youtrack ticket with a link to the corresponding Gitlab-Commit.

[![Build status](https://ci.appveyor.com/api/projects/status/h2jxd49dktnb903c?svg=true)](https://ci.appveyor.com/project/brase/youtrackgitlab)
[![Build Status](https://travis-ci.org/rheinspree/YoutrackGitlab.svg?branch=master)](https://travis-ci.org/rheinspree/YoutrackGitlab)
