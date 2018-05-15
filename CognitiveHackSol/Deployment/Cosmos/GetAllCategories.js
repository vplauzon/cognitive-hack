function getAllCategories() {
    var response = getContext().getResponse();
    var collection = getContext().getCollection();
    var query = `
SELECT cat.name
FROM c
JOIN cat in c.categories
WHERE c.objectType = "image"
`;
    var countDict = {};

    tryQuery();

    function tryQuery(continuation) {
        var requestOptions = { continuation: continuation };
        // Query documents for all categories
        var isAccepted = collection.queryDocuments(
            collection.getSelfLink(),
            query,
            requestOptions,
            function (err, feed, responseOptions) {
                if (err) {
                    throw err;
                }

                if (feed) {
                    for (var i = 0; i != feed.length; ++i) {
                        var name = feed[i].name;

                        if (countDict[name]) {
                            ++(countDict[name]);
                        }
                        else {
                            countDict[name] = 1;
                        }
                    }
                }

                if (responseOptions.continuation) {
                    tryQuery(responseOptions.continuation)
                } else {
                    response.setBody(countDict);
                }
            });

        if (!isAccepted) {
            throw new Error('The query was not accepted by the server.');
        }
    }
}