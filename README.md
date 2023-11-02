# LetChooseTeams



#V2

Version prepared to run with k8s infrastructure.

## Infrastructure 

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2-k8s.png)

Application is managed by k8s with load balancer service with hpa scalability above 70% cpu utilization.

## Backend Architecture



## Project structure

### SetUp
Working on local docker registry

`docker run -d -p 7000:5000 --restart=always --name registry registry:2`

To 'push' to registry

`docker build . -t localhost:7000/lct`

`docker push localhost:7000/lct` 

#V1



![alt text](https://github.com/AGranosik/LetChooseTeams/blob/event-sourcing/images/v1_architectures.png)

Frontent communicate with Backend via http requests.
WebSockets added for better responsivness & User experience


TO DO: 
[X] docker
[X] k8s for localhost
[X] k6 tests
[X] check if code does not need refactor in some places
[X] k6 tests
[-] environment for k6 tests
[X] websockets
[X] websockets should be in infra layer
[X] Cancellation token
[X] can easly remove actions from service -> refactor if need to
[X] Redis
[X] poprawka działania fe z be
[X] publish fallbacks
[X] test for redis meesage beroker
[X] redis shouldnt block requests
[X] check logs
[X] update to .net core 7
[X] udpate packages
[X] work on mobile
[X] swagger documentation
[X] return request id
[X] documentation should return request id model
[X] routing fe -> remove play names from params and store in store 
[X] generation qr code -> fe?
[X] update k8s
[X] k6 tests
[X] load scalling
[X] health chcecks
[X] api versioning - concept
[X] api versioning -> simple implementation
[X] need 3 isntances of mongo? -- some local files
[] documentation
[] hpa with memory utilization & stabilizationWindowSeconds
[] azure
[] azure monitoring
???[]action microservice

What documenttion should include:
[] why cancellation token is only in on request -> to shwo how to use for heavy loaded endpoints only
[] explanation for tests library
[] swagger documentation
[] Redis