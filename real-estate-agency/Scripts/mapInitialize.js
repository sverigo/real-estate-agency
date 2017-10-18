// Google Maps API Key: AIzaSyDs0eQvxU2jsqf71-Smlevn-NsX8W_V74g
var defaultCoords = { lat: 58.47, lng: 35.05 }; // Dnipro coordinates
// Setting parameters and call geocode func
function initMap() {
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 12,
        center: defaultCoords
    });
    var defaultMarker = new google.maps.Marker({
        map: map,
        animation: google.maps.Animation.DROP,
        position: defaultCoords
    });
    var geocoder = new google.maps.Geocoder();
    
    geocodeAddress(geocoder, map);
}
// Getting the address from fields
function geocodeAddress(geocoder, resultsMap) {
    var address = document.getElementById('Address').innerHTML;
    
    geocoder.geocode({ 'address': address }, function (results, status) {
        if (status === 'OK') {
            resultsMap.setCenter(results[0].geometry.location);
            var marker = new google.maps.Marker({
                map: resultsMap,
                animation: google.maps.Animation.DROP,
                position: results[0].geometry.location
            });
        } else {
            alert('Geocode was not successful for the following reason: ' + status);
        }
    });
}

