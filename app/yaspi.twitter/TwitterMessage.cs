namespace yaspi.twitter;
public class TwitterMessage
{
    public int TwitterMessageId {get;set;}
    public string Id { get; set; }
    public string Text {get;set;}
    public DateTime CreatedAt {get;set;}

}

// {
        // "data": {
        //     "edit_history_tweet_ids": [
        //         "1626472722376056832"
        //     ],
        //     "id": "1626472722376056832",
        //     "text": "Hello World!"
        // }
    //}