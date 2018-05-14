function search_bindCriteria(tagCount, imageResult, imageResultTemplate) {
    var criteria = { tags: [] };
    var searchState = {
        isRefreshing: false,
        isOneWaiting: false,
        criteria: criteria,
        controls: {
            imageResult: imageResult,
            imageResultTemplate:  imageResultTemplate
        }
    };

    for (var i = 0; i != tagCount; ++i) {
        var inputBox = document.getElementById("tagCheckBox" + i);

        search_injectOnClick(searchState, inputBox);
    }
    //  Perform the initial search
    search_onChangeCriteria(searchState);
}

function search_injectOnClick(searchState, inputBox) {
    inputBox.onclick = function () {
        if (inputBox.checked) {
            var index = searchState.criteria.tags.findIndex(function (v) { return v == inputBox.value; });

            if (index == -1) {
                searchState.criteria.tags.push(inputBox.value);
            }
        }
        else {
            var index = searchState.criteria.tags.findIndex(function (v) { return v == inputBox.value; });

            if (index >= 0) {
                searchState.criteria.tags.splice(index, 1);
            }
        }

        search_onChangeCriteria(searchState);
    }
}

function search_onChangeCriteria(searchState) {
    if (searchState.isRefreshing) {
        searchState.isOneWaiting = true;
    }
    else {
        searchState.isRefreshing = true;
        searchState.isOneWaiting = false;
        searchApiProxy_search(
            searchState.criteria,
            function (result) {
                search_displayResults(result, searchState.controls);
                searchState.isRefreshing = false;
                if (searchState.isOneWaiting) {
                    search_onChangeCriteria(searchState);
                }
            },
            function () {
                searchState.isRefreshing = false;
                searchState.isOneWaiting = false;
            })
    }
}

function search_displayResults(result, controls) {
    //  Clear results
    controls.imageResult.innerHTML = "";

    for (var i = 0; i < result.length; ++i) {
        var clone = controls.imageResultTemplate.cloneNode(true);

        search_appendIds(clone, i);
        controls.imageResult.appendChild(clone);

        var thumbnail = document.getElementById('thumbnail' + i);
        var tooltip = document.getElementById('tooltip' + i);
        var text = result[i].captions.join('\n')
            + '('
            + result[i].categories.join(', ')
            + ')'
            + '{'
            + result[i].tags.join(', ')
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