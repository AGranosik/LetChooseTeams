# LetChooseTeams


##To run project:
`docker-compose up -d`
Set api project as startup project
`dotnet run`

## Install third-tool to run performance tests
> docker pull loadimpact/k6

# Run tests
> docker run -i loadimpact/k6 run - <script.js



#V1
Core architecture of system.

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/event-sourcing/images/v1_architecture.svg)

Frontent communicate with Backend via http requests.
WebSockets added for better responsivness & User experience