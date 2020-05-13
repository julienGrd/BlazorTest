function synchronizeTableScroll(containerTableId, onTop) {
    var $element = $('#' + containerTableId + "> table > tbody > tr.selected-element");
    if ($element.length) {
        $element.get(0).scrollIntoView(onTop);
        //if (document.getElementById(containerTableId).scrollTop != 0) {
        //    document.getElementById(containerTableId).scrollTop += 30;
        //}

    }
}

var _list1 = null;
var _list2 = null;
var _list3 = null;
var _currentPage = null;

function setPage(page) {
    _currentPage = page;

    $(".page-selector").removeClass("selected");
    $("#pageLink" + page).addClass("selected");

    $("#tableFetchDataJs > tbody > tr").remove();

    var currentList = null;
    if (page == 1) {
        currentList = _list1;
    }
    else if (page == 2) {
        currentList = _list2;
    }
    else if (page == 3) {
        currentList = _list3;
    }
    if (currentList) {
        var tableObj = $("#tableFetchDataJs > tbody");
        currentList.forEach(element => {
            tableObj.append($('<tr>')
                .append($('<td>')
                    .append(element.date)
            )
                .append($('<td>')
                    .append(element.temperatureC)
            )
                .append($('<td>')
                    .append(element.temperatureF)
            )
                .append($('<td>')
                    .append(element.summary)
            )
                .append($('<td>')
                    .append(element.city.name)
                )
            );

        });
    }
    
}

function manageFetchDataJs(list1, list2, list3) {
    _list1 = list1;
    _list2 = list2;
    _list3 = list3;

    setPage(1);
}

window.VirtualizedComponent = {
    _ticking: false,
    _initialize: function (component, contentElement) {
        // Find closest scrollable container
        let scrollableContainer = contentElement.parentElement;
        while (!(scrollableContainer.style.overflow || scrollableContainer.style.overflowY)) {
            scrollableContainer = scrollableContainer.parentElement;
            if (!scrollableContainer) {
                throw new Error('No scrollable container was found around VirtualizedComponent.');
            }
        }

        // TODO: Also listen for 'scrollableContainer' being resized or 'contentElement' moving
        // within it, and notify.NET side
        scrollableContainer.addEventListener('scroll', e => {
            const lastKnownValues = {
                containerRect: scrollableContainer.getBoundingClientRect(),
                contentRect: readClientRectWithoutTransform(contentElement)
            };

            if (!this._ticking) {
                requestIdleCallback(() => {
                    component.invokeMethodAsync('OnScroll', lastKnownValues);
                    this._ticking = false;
                });

                this._ticking = true;
            }
        });

        return {
            containerRect: scrollableContainer.getBoundingClientRect(),
            contentRect: readClientRectWithoutTransform(contentElement)
        };
    }
};

function readClientRectWithoutTransform(elem) {
    const rect = elem.getBoundingClientRect();
    const translateY = parseFloat(elem.getAttribute('data-translateY'));
    return { top: rect.top - translateY, bottom: rect.bottom - translateY, left: rect.left, right: rect.right, height: rect.height, width: rect.width };
}
