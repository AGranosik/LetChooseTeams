# LetChooseTeams



#V2

Working on local docker registry

`docker run -d -p 7000:5000 --restart=always --name registry registry:2`

To 'push' to registry

`docker build . -t localhost:7000/lct`



#V1

##To run project:
`docker-compose up -d` 
Set api project as startup project
`dotnet run`

## Install third-tool to run performance tests
> docker pull loadimpact/k6

# Run tests
> docker run -i loadimpact/k6 run - <script.js

Core architecture of system.

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/event-sourcing/images/v1_architectures.png)

Frontent communicate with Backend via http requests.
WebSockets added for better responsivness & User experience


TO DO: 
[X] docker
[X] k8s for local host
[X] k6 tests
[X] check if code does not need refactor in some places
[X] k6 tests
[-] environment for k6 tests
[X] websockets
[] Redis
[] health chcecks
[] api versioning
[] publish fallbacks
[] true cqrs
[] documentation
[] monitoring for memory leaks
???[]action microservice
[] azure
[] azure monitoring