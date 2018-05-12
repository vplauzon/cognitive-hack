function getAllCaptions() {
    var collection = getContext().getCollection();

    // Query documents for all categories
    var isAccepted = collection.queryDocuments(
        collection.getSelfLink(),
        `
SELECT caption.text
FROM c
JOIN caption in c.captions
WHERE c.objectType = "image"
        `,
        function (err, feed, options) {
            if (err) {
                throw err;
            }
            var response = getContext().getResponse();

            if (!feed) {
                response.setBody('No feed provided');
            }
            else {
                var countDict = {};

                for (var i = 0; i != feed.length; ++i) {
                    var name = feed[i].text;

                    if (countDict[name]) {
                        ++(countDict[name]);
                    }
                    else {
                        countDict[name] = 1;
                    }
                }

                response.setBody(countDict);
            }
        });

    if (!isAccepted) {
        throw new Error('The query was not accepted by the server.');
    }
}