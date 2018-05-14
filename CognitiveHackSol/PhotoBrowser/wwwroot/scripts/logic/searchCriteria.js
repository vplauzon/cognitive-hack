function searchCriteria_bindCriteria(tagCount) {
    var criteria = { tags: [] };

    for (var i = 0; i != tagCount; ++i) {
        var inputBox = document.getElementById("tagCheckBox" + i);

        searchCriteria_onClick(criteria, inputBox);
    }
}

function searchCriteria_onClick(criteria, inputBox) {
    inputBox.onclick = function () {
        alert("click " + inputBox.value + ':  ' + inputBox.checked);
        if (inputBox.checked) {
            var index = criteria.tags.findIndex(function (v) { return v == inputBox.value; });

            if (index == -1) {
                criteria.tags.push(inputBox.value);
            }
        }
        else {
            var index = criteria.tags.findIndex(function (v) { return v == inputBox.value; });

            if (index >= 0) {
                criteria.tags.splice(index, 1);
            }
        }

        searchCriteria_onChangeCriteria(criteria);
    }
}

function searchCriteria_onChangeCriteria(criteria) {
    alert(JSON.stringify(criteria));
}