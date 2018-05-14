function bootstrapSearch(tagCount) {
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');

    searchCriteria_bindCriteria(tagCount);
    search_searchNoCriteria(imageResult, imageResultTemplate);
}