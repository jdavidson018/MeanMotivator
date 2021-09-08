# Creating a web api running .Net 5 and Mongo DB on docker and Linux web server

## Step 1: Create the most basic api possible
1. Create the model class
    * Here it is Comment.cs
2. Create Repository Interface
    * ICommentsRepository.cs
    * We create an interface so that we can quickly implement different repositories that are all compatible with our controller. So if we ever switch to another database provider (which we are going to do), we do not have to modify the controller at all.
3. Create an InMemory repository
    * InMemCommentsRepository.cs
    * This is implemented asynchronously even though it probably doesn't need to be. It is still good practice to do it that way
4. Create controller
    * CommentsController.cs
    1. Create Dtos to support controller at the same time
        * CreateCommentDto.cs
        * ItemDto.cs
        * UpdateItemDto.cs
    2. Add asDto in Extentions.cs
5. Add InMemory Repository to the startup class
    ```
    services.AddControllers(options => {
        options.SuppressAsyncSuffixInActionNames = false;
    });
    ```
6. Suppress Async suffix (also in startup class)
    ```
    services.AddSingleton<ICommentsRepository, InMemCommentsRepository>();
    ```
7. Stop api from opening a new browser tab by removing the following from ```launch.json```
    ```
    // Enable launching a web browser when ASP.NET Core starts. For more information: https://aka.ms/VSCode-CS-LaunchJson-WebBrowser
    "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
    },
    ```
8. Test in postman!

## Step 2. Creating a MongoDB backend (Docker comes in here)
1. Create the MongoDB repository class
    ```MongoDbCommentsRepository```
2. Install Required Mongo DB Nuget Packages
    * ```MongoDb.Driver```
    * ```AspNetCore.HealthChecks.MongoDb```
    1. Restore after installing the dependencies
3. Setup a MongoDB Backent
    1. Start up a docker container
        ```docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD={password} mongo```
    2.  Save the password for the container as a secret
        ```dotnet user-secrets init```
        ```dotnet user-secrets set MongoDbSettings:Password {value}``` 
        ```dotnet user-secrets set TwilioAuthToken {token}```
    3. Create a settings file that will be used to populate connection information from the appsettings file
        ```Settings.MongoDbSettings.cs```
    4. Add the rest of the DB settings to the app settings
        ```
        "MongoDbSettings":{
            "Host" : "localhost",
            "Port" : "27017",
            "User" : "mongoadmin"
        }
        ```
    5. Add the following to Startup.cs
        ```
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));
        var mongoDbSettings = Configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();

        services.AddSingleton<IMongoClient>(serviceProvider => 
        {
            // This is a cool way to construct an object from a setting an the appsettings file
            return new MongoClient(mongoDbSettings.ConnectionString);
        });
        
        // First is the interface, second is the actual implementation of it.
        services.AddSingleton<IItemsRepository, MongoDbItemsRepository>();
        ```

## Step 3: Add Health Checks

1. In startup.cs configureservices, add the following:
    ```
    services.AddHealthChecks()
    .AddMongoDb(
        mongoDbSettings.ConnectionString,
        name: "mongodb", 
        timeout: TimeSpan.FromSeconds(3),
        tags: new[] {"ready"});
    ```

2. In startup.cs configure, add the following:
    ```
    app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks("/health/ready", new HealthCheckOptions{
                Predicate = (check) => check.Tags.Contains("ready"),
                ResponseWriter = async(context, report) => 
                {
                    var result = JsonSerializer.Serialize(
                        new{
                            status = report.Status.ToString(),
                            checks = report.Entries.Select(entry => new {
                                name = entry.Key,
                                status = entry.Value.Status.ToString(),
                                exception = entry.Value.Exception != null ? entry.Value.Exception.Message : "none",
                                duration = entry.Value.Duration.ToString()
                            })
                        }
                    );
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsync(result);
                }
            });
            endpoints.MapHealthChecks("/health/live", new HealthCheckOptions{
                Predicate = (_) => false
            });          

        });
    ```
3. Add the endpoint to postman

## Creating an image for API
1. Create a docker file using the vscode extension
2. Build docker image

    ```docker image build -t jdavid123/meanmotivator:v1 .```
    * List with ```docker image ls```
3. Push image to docker hub

    ```docker image push jdavid123/meanmotivator:v1```

## Setting up without kubernetes
1. Create a network for docker
    ```docker network create meanmotivatornet```
    * list with ```docker network ls```
2. Selectively shut off https redirection
    ```
    if(env.IsDevelopment()){
        app.UseHttpsRedirection();
    }
    ```
2. Stop initial mongo
    ```docker stop mongo```
3. Start new mongo container on the correct network
    ```docker run -d --rm --name mongo -p 27017:27017 -v mongodbdata:/data/db -e MONGO_INITDB_ROOT_USERNAME=mongoadmin -e MONGO_INITDB_ROOT_PASSWORD={from bit} --network=meanmotivatornet mongo```
4. Run API image on the correct network
```docker run -it --rm --name meanmotivator -p 8080:80 -e MongoDbSettings:Host=mongo -e MongoDbSettings:Password={from bit} -e TwilioAuthToken={From console} --network=meanmotivatornet jdavid123/meanmotivator:v2```
5. At this point, you will have to run the endpoints using http, you can change the port to 8080.

***I am completely ignoring kubernetes at this point. To run on scout, you just need to clone the repository and create the image on the server because apple silicon goofs it up.***
## Running on Scout web server
1. Clone the repository on scout
    ```git clone https://github.com/jdavidson018/MeanMotivator.git```
2. ```cd MeanMotivator```
3. Install kubernetes if not already done
    ```https://ubuntu.com/kubernetes/install```
4. Dashboard is not working...
5. Run command from section above but prefix with microk8s

## Updating the project
1. Commit code
2. Pull on the server, rebuild and push the image.
3. Build the image again
```docker run -it --rm -p 8080:80 -e MongoDbSettings:Host=mongo -e MongoDbSettings:Password={from bit} --network=meanmotivatornet jdavid123/meanmotivator:v2```