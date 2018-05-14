function search_bindCriteria(tags, imageResult, imageResultTemplate, searchCount) {
    var criteria = { tags: [] };
    var searchState = {
        isRefreshing: false,
        isOneWaiting: false,
        criteria: criteria,
        controls: {
            imageResult: imageResult,
            imageResultTemplate: imageResultTemplate,
            searchCount: searchCount
        }
    };

    tags.forEach(inputBox => search_injectOnClick(searchState, inputBox));

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
    var images = result.images;

    //  Clear results
    controls.imageResult.innerHTML = "";
    controls.searchCount.textContent = result.totalAvailable;

    for (var i = 0; i < images.length; ++i) {
        var clone = controls.imageResultTemplate.cloneNode(true);

        search_appendIds(clone, i);
        controls.imageResult.appendChild(clone);

        var thumbnail = document.getElementById('thumbnail' + i);
        var tooltip = document.getElementById('tooltip' + i);
        var text = images[i].captions.join('\n')
            + '('
            + images[i].categories.join(', ')
            + ')'
            + '{'
            + images[i].tags.join(', ')
            + '}';

        thumbnail.src = images[i].thumbnailUrl;
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