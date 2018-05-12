function searchCriteria_bootstrap(tagList, tagTemplate) {
    groupApiProxy_getAllCaptions(
        function (result) {
            searchCriteria_displayTags(result, tagList, tagTemplate);
        },
        function () {
        })
}

function searchCriteria_displayTags(result, tagList, tagTemplate) {
    //  Clear tag list
    tagList.innerHTML = "";

    for (var i = 0; i < result.length; ++i) {
        var clone = tagTemplate.cloneNode(true);

        searchCriteria_appendIds(clone, i);
        tagList.appendChild(clone);

        var tagText = document.getElementById('tagText' + i);
        var text = result[i].name
            + ' ('
            + result[i].count
            + ')';

        tagText.innerHTML = text;
        clone.style.display = 'block';
    }
}

function searchCriteria_appendIds(element, index) {
    element.id += index;

    for (var i = 0; i < element.children.length; ++i) {
        search_appendIds(element.children[i], index);
    }
}