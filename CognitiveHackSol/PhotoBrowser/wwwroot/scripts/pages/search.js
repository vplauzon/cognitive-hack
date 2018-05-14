function bootstrapSearch(tagCount) {
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');

    search_bindCriteria(tagCount, imageResult, imageResultTemplate);
}