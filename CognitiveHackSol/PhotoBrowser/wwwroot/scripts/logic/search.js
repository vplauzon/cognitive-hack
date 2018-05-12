﻿function search_searchNoCriteria(sessionId, imageResult, imageResultTemplate) {
    searchApiProxy_searchNoCriteria(
        sessionId,
        function (result) {
            search_displayResults(result, imageResult, imageResultTemplate);
        },
        function () {
        })
}

function search_displayResults(result, imageResult, imageResultTemplate) {
    //  Clear results
    imageResult.innerHTML = "";

    for (var i = 0; i < result.length; ++i) {
        var clone = imageResultTemplate.cloneNode(true);

        search_appendIds(clone, i);
        imageResult.appendChild(clone);

        var thumbnail = document.getElementById('thumbnail' + i);
        var tooltip = document.getElementById('tooltip' + i);
        var text = result[i].captions.join('\n')
            + '\n'
            + '{'
            + result[i].categories.join(', ')
            + '}';

        thumbnail.src = result[i].thumbnailUrl;
        tooltip.innerHTML = text;
        clone.style.display = 'block';
    }
}

function search_appendIds(element, index) {
    element.id += index;

    for (var i = 0; i < element.children.length; ++i) {
        search_appendIds(element.children[i], index);
    }
}