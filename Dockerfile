FROM mono

RUN apt-get update
RUN apt-get install -y fsharp

WORKDIR /app
ADD . /app
RUN /app/build.sh

ENTRYPOINT ["/usr/bin/mono", "/app/build/YoutrackGitlab/YoutrackGitlab.exe"]
