function search_searchNoCriteria(sessionId, photoText) {
    searchApiProxy_searchNoCriteria(
        sessionId,
        function (payload) {
            photoText.value = JSON.stringify(payload);
        },
        function () {
        })
}