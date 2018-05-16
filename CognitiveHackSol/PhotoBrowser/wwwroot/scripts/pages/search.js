function bootstrapSearch(tagCount, categoryCount, captionCount) {
    var imageResult = document.getElementById('imageResult');
    var imageResultTemplate = document.getElementById('imageResultTemplate');
    var searchCount = document.getElementById('searchCount');
    var tagRange = Array(tagCount).fill(0).map((x, y) => x + y);
    var tags = tagRange.map((i) => document.getElementById("tagCheckBox" + i));
    var categoryRange = Array(categoryCount).fill(0).map((x, y) => x + y);
    var categories = categoryRange.map((i) => document.getElementById("categoryCheckBox" + i));
    var captionRange = Array(captionCount).fill(0).map((x, y) => x + y);
    var captions = captionRange.map((i) => document.getElementById("captionCheckBox" + i));

    search_bindCriteria(tags, categories, captions, imageResult, imageResultTemplate, searchCount);
}