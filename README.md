# LetChooseTeams

##To run project:
`docker-compose up -d`
Set api project as startup project
`dotnet run`

## Install third-tool to run performance tests
> docker pull loadimpact/k6

# Run tests
> docker run -i loadimpact/k6 run - <script.js