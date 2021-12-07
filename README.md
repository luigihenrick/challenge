# HackerNews BestStories Challenge

This project it's a challenge to list best 20 hacker news stories from [HackerNews API](https://github.com/HackerNews/API) and can be tested [here](https://luigihenrick-challenge.herokuapp.com/best20).

## How to run

```
dotnet restore
dotnet run --project ./API/API.csproj
```

### BaseUri
You can change the HackerNews base uri from **appsettings.json**

```
"HackerNews":{
    ...Other configs
    "BaseUri": "https://hacker-news.firebaseio.com/"
}
```

### Cache
This application has in-memory cache that can be configured altering the **appsettings.json**

```
"HackerNews":{
    ...Other configs
    "Cache": {
        "AbsoluteExpiration": 360, // Change here cache time
        "SlidingExpiration": 5     // Change here cache time
    }
}
```

### Polly
This application uses polly for retry and can be configured altering the **appsettings.json**

```
"HackerNews":{
    ...Other configs
    "Polly": {
        "MaxRetryCount": 3,             // Change here the retry count
        "SleepDurationInSeconds": 5     // Change here the sleep duration beetwen requests
    }
}
```