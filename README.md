# LetChooseTeams

Simple web application to help orginize FIFA 23 tournament. Where player join to tournament by scanning QR Code, provide player info data then select a team.
When all players chose team team are assigned to each player randomly.


# V2

Version prepared to run with k8s infrastructure. Working on multi instances.

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

[More on](https://www.mongodb.com/docs/manual/replication/)

Decided to use mongoDB to get new knowledge on no-sql database.

## Application Architecture

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/v2_architecture.png)

---


## Project structure

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/clean_architecture.png)

---

Higher layer (more outside) is implementing interfaces provider by those lower (closer to the center).

### Infrastructure layer

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/infrastructure_class_diagram.png)

---

Two main roles of infrstructrure layer in my application:
1. Persistent data.

	To store events i decided to use Mongo database.

	Some issues i decided to solve:
	* Field uniqness

	Solution for that are mongo db indexes but decided to create more generic way to store events and achive uniqness for simple db fields.
	We just have to implement `IUniqness` and call method in MongoPersistanceClient.Configure() method.

	```c#
		ConfigureFieldUniqness<Tournament, SetTournamentNameEvent>(uniqueIndexOptions);
	```

	* 'Coinditional' versioning

	Some events may increase Entity version because of their impact on application flow. Changing tournament's name is essential and we do not want these changes to be override by other party (two request simultaneously).
	Other events (like selecting team) does not increase tournament version because we want to work it simultaneously and selecting team by one player does not impact team selection by the other (as long as its not same team)

	We just need to implement `IVersionable` interface and repository will increase entity version.

	* Snapshots
	It's repository responsibility to create snapshot to increase perfomance. These are created because we do not always want create every aggregate from scratch by applying every event one after another but just from valid point.
	I aware that my application would be fine wihtout it and maybe even better. It's just for learining puroposes.
	
```c#
    public class SetTournamentNameEvent : DomainEvent, IVersionable, IUniqness
    {
        public string TournamentName { get; set; }
        public string UniqueValue { get => TournamentName; }

        public SetTournamentNameEvent(string tournamentName, Guid streamId) : base(streamId)
        {
            TournamentName = tournamentName;
        }
    }
```

2. Manage websocket messages. 

To receive or send any web socket message ```Hub``` had to be configured.
``` endpoints.MapHub<TournamentHub>("/hubs/actions"); ```

After message is received we have to publish it on Redis Pub/Sub to notify other pods then process the message.

```c#
        public async Task PublishAsync<T>(string groupId, T message)
        {
            var clients = await TryPublishAsync(groupId, message);
        }

        public async Task SubscribeAsync(MessageBrokerConnection connection)
        {
            if (!ConnectionValidation(connection))
                return;

            var connections = GetConnectionsIfGroupsExists(connection.GroupId);
            if(connections is null)
            {
                _groupConnectionsDicitonary.Add(connection.GroupId, new List<string> { connection.UserIdentifier });
                await _retryWrappedPolicy.ExecuteAsync(async () => {
                    var group = GetConnectionsIfGroupsExists(connection.GroupId);
                    if(group is not null)
                        await SubscribeAsync(connection.GroupId);
                });
            }
            else
            {
                connections.Add(connection.UserIdentifier);
            }
        }
```

### Application layer

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/application_layer_part.png)

---

Implementing *Command Query Responsibility Segregation (CQRS)*  pattern.

### Domain

---

![alt text](https://github.com/AGranosik/LetChooseTeams/blob/main/images/v2/domain.png)

---

Domain class structure.

## SetUp

* In main folder run followed command to setup local environment for project

`docker-compose up -d`

* Working on local docker registry

`docker run -d -p 7000:5000 --restart=always --name registry registry:2`

* Build then push backend app into registry

`docker build ./LCT -t localhost:7000/lct`

`docker push localhost:7000/lct` 


K8s configuration

* k8s namespace
`kubectl create namespace lct-namespace`

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
[X] documentation
[X] logs message template
[X] ConfigureAwait(false) -> read and implement probably not for web apps
[X] redis failure fallbacks
[X] multiple user with fewer request (more real example)
[X] k6 tests on single pod [localhost]
[X] save k6 results for comparision with k8s pods
[X] cancellation tokens & compare tests
[X] k6 tests on single pod
[] hpa with memory utilization & stabilizationWindowSeconds & update documetation
[X] k6 on k8s
[] azure
[] azure monitoring
?[] setup env for k6 tests


Azure chapter
mongo:
	[X] setup db
	[X] store credential in safe way
	[] change mongo version to 6.0
	?[] script for creation mongo db on azure
	
[X] aks vs app service vs containarized app\
[] setup redis & elastic & kibana
[X] push to docker hub
[X] setup aks & get access to swagger
[] env variables for Project from Azure
[X] variables in pipeles