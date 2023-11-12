# LetChooseTeams



#V2

Version prepared to run with k8s infrastructure.

## Infrastructure 

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2-k8s.png)

Application is managed by k8s with load balancer service with hpa scalability above 70% cpu utilization.

Purpose of that was to handle heavy load with high availability.

* LoadBalancer -  ensuring that incoming requests are evenly distributed among the available instances to optimize performance and prevent overload on any single instance, providing high availability and scalability.
* HorizontalPodAutoscaler (hpa) - automatically updates a workload resource with the aim of automatically scaling the workload to match demand.
* Deployment - describe desired state of backend application

---

### Logs

Solution using **Elasticsearch + Kibana** for logging and aggregation.

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2-logs.png)

---

Each log contains fields which help to observe application behaviour based on response times or error occurences.

* **Elapsed** [ms] - How much time has passed since start fo request.
* **RequestId** - Unique Id [Guid] per request which helps to track logs for concrete request. Id is returned by server when error occurs.
* **RequestPath** - endpoint
* **StatusCode** - http response code

Log fields may differ depends in which place logger is called. Not in every log field 'StatusCode' will appear because during request processing we don't know response code yet.

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2-visualize.png)

---


### Redis Pub/Sub

---

Implementing Publish/Subscribe messagin pattern which helps to handle request via websockets.
Each message received from client is published to channel. Every instance (Pod) subsribe to a channel whenever Websocket connection is established.
This solution enable to scale instances while working with ws connections.

![redis](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2_redis.png)

---

### Mongo

Document database with replica set configuration. Which provides redundancy and high availability.

[More on ](https://www.mongodb.com/docs/manual/replication/)


## Application Architecture

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2_architecture.png)

---


## Project structure

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/clean_architecture.png)

---

### SetUp

* In main folder run followed command to setup local environment for project

`docker-compose up -d`

* Working on local docker registry

`docker run -d -p 7000:5000 --restart=always --name registry registry:2`

* Build then push backend app into registry

`docker build ./LCT -t localhost:7000/lct`

`docker push localhost:7000/lct` 


K8s configuration

* Setup Deployment
`kubectl apply -f .\deployment.yaml`

* LoadBalancer
`kubectl apply -f .\loadBalancer.yaml`

* metric service - essential for hpa to work
`kubectl apply -f .\metric-service.yaml`

* Setup hpa
`kubectl apply -f .\hpa.yaml`

##### Swagger available at
` http://localhost:6008/swagger/index.html `


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
[] redis failure fallbacks
[] tests
[] message templates for groupping
[] hpa with memory utilization & stabilizationWindowSeconds & update documetation
[] k6 tests on single pod [localhost]
[] k6 on k8s
[] azure
[] azure monitoring