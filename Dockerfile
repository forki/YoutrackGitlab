FROM mono

RUN apt-get update
RUN apt-get install -y fsharp

WORKDIR /app
ADD . /app
RUN /app/build.sh

