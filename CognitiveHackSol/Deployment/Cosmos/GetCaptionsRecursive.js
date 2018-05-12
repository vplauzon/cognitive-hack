function getCaptionsRecursive(countDict) {
    var collection = getContext().getCollection();

    // Query documents and take 1st item.
    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        'SELECT * FROM root r',
        function (err, feed, options) {
            if (err) {
                throw err;
            }

            // Check the feed and if empty, set the body to 'no docs found', 
            // else take 1st element from feed
            if (!feed || !feed.length) {
                var response = getContext().getResponse();
                response.setBody('no docs found');
            }
            else {
                var response = getContext().getResponse();
                var body = { prefix: "Johnny", feed: feed[0] };
                response.setBody(JSON.stringify(body));
            }
        });

    if (!isAccepted) {
        throw new Error('The query was not accepted by the server.');
    }
}
/*
    SELECT TOP 1 cat.name
    FROM c
    JOIN cat in c.categories
    WHERE c.objectType='image'
    AND cat.name <> "people_portrait"
    AND cat.name <> "people_young"

    SELECT VALUE COUNT(1)
    FROM c1
    JOIN cat in c1.categories
    WHERE c1.objectType='image'
    AND cat.name = "people_portrait"
*/