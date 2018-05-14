function bootstrapSearch(tagCount) {
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');
    var searchCount = document.getElementById('searchCount');
    var tagRange = Array(tagCount).fill(0).map((x, y) => x + y);
    var tags = tagRange.map((i) => document.getElementById("tagCheckBox" + i));

    search_bindCriteria(tags, imageResult, imageResultTemplate, searchCount);
}