$(document).ready(function () {
    $("a.group-item").fancybox({
        'type': 'image',
        'maxWidth': 800,
        'maxHeight': 600,
        'width': '70%',
        'height': '70%',
        'autoSize': false,
        'transitionIn': 'elastic',
        'transitionOut': 'elastic',
        'speedIn': 600,
        'speedOut': 200,
        'overlayShow': false
    });
});