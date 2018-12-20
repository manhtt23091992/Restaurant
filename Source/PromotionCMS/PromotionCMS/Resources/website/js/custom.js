var deviceIsMobile = false; //At the beginning we set this flag as false. If we can detect the device is a mobile device in the next line, then we set it as true.
if (/(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|ipad|iris|kindle|Android|Silk|lge |maemo|midp|mmp|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino/i.test(navigator.userAgent) ||
   /1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-/i.test(navigator.userAgent.substr(0, 4))) {
   deviceIsMobile = true;
}

var closedCSS = { 'max-height': 0, 'overflow': 'hidden' };
var breakpointMenu = 900; //breakpoint of menu
var iscrollMenuEnable = false; //Enable iscroll for menu;
var getPositionChangeOpacityPara = 2;
var menuSettings = {
   breakpoint: 900, //breakpoint of menu
   iscrollMenuEnable: false, //Enable iscroll for menu;
   itemsAnimationDelay: 50 //Item tranision ios
}

var opacitySpeed = 80;
$(function () {
   $('.select-js').mobiscroll().select({
      showScrollArrows: true,
      theme: 'ios',
      display: 'bottom',
      label: 'City',
      group: true,
      groupLabel: 'List',
      placeholder: $(this).attr('data-placeholder') + '',
      filter: true,
      multiline: 2,
      display: $(window).width() < 480 ? 'bottom' : 'bubble',
      lang: "vi",
      inputClass: "input-search",
      rows: 5,
      filterPlaceholderText: "Tìm kiếm...",
      buttons: [{
         text: 'Đóng',
         handler: 'cancel',
      }],
      onBeforeShow: function (event, inst) {
         console.log(inst)
         if ($(window).width() > 480) {
            $('body').css('position', 'static')
         }
      },
      onClose: function (event, inst) {
         $('body').css('position', '')
      }
   });
});
//google.maps.event.addDomListener(window, 'load', initialize);

//function initialize() {
//   var data = [
//      ['Vnpay', 'http://toanhavietnam.com/wp-content/uploads/2017/02/TDL.jpg', '22 Láng Hạ', 'images/pinblue.svg', 'hot'],
//      ['Vietcombank', 'https://vneconomy.mediacdn.vn/zoom/500_312/Xcqmf7oTLBAljrydzccccccccccccz/Image/2016/12/0-85737.jpg', '140-142 Lê Lợi, Hải Châu 1, Hải Châu, Đà Nẵng, Vietnam', 'images/pinblue.svg', ' '],
//      ['Sacombank', 'http://sohanews.sohacdn.com/zoom/700_438/2017/photo1498976726775-1498976727015-0-27-314-533-crop-1498976805146.jpg', '266 - 268 Nam Kỳ Khởi Nghĩa, phường 8, Hồ Chí Minh', 'images/pinred.svg', ' '],
//      ['Shop Mẹ và Bé', 'http://cdn.nhanh.vn/cdn/store/26/art/article_1488777663_170.jpg', '29 Láng Hạ', 'images/pinblue.svg', ' '],
//      ['Shop Bố và Bé', 'http://cdn.nhanh.vn/cdn/store/26/art/article_1488777663_170.jpg', '29 Thái Hà', 'images/pinblue.svg', 'hot'],
//      ['Shop Chị và Bé', 'http://cdn.nhanh.vn/cdn/store/26/art/article_1488777663_170.jpg', '114 Láng Hạ', 'images/pinred.svg', ' '],
//   ];
//   var mapOptions = {
//      zoom: 6,

//      // The latitude and longitude to center the map (always required)
//      center: new google.maps.LatLng(16.509833, 105.996094), // New York

//      // styles1: [{ "featureType": "all", "elementType": "labels.text.fill", "stylers": [{ "saturation": 36 }, { "color": "#333333" }, { "lightness": 40 }] }, { "featureType": "all", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "on" }, { "color": "#ffffff" }, { "lightness": 16 }] }, { "featureType": "all", "elementType": "labels.icon", "stylers": [{ "visibility": "off" }] }, { "featureType": "administrative", "elementType": "geometry.fill", "stylers": [{ "color": "#fefefe" }, { "lightness": 20 }] }, { "featureType": "administrative", "elementType": "geometry.stroke", "stylers": [{ "color": "#fefefe" }, { "lightness": 17 }, { "weight": 1.2 }] }, { "featureType": "administrative", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "off" }] }, { "featureType": "landscape", "elementType": "geometry", "stylers": [{ "color": "#f5f5f5" }, { "lightness": 20 }] }, {
//      //         "featureType": "administrative.country",
//      //         "elementType": "geometry.stroke",
//      //         "stylers": [{
//      //             "color": "#a7a7a7"
//      //         }]
//      //     }, {
//      //         "featureType": "administrative.province",
//      //         "elementType": "geometry.stroke",
//      //         "stylers": [{
//      //             "color": "#aeaeae"
//      //         }]
//      //     },
//      //     { "featureType": "landscape", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "off" }] }, { "featureType": "poi", "elementType": "geometry", "stylers": [{ "color": "#f5f5f5" }, { "lightness": 21 }] }, { "featureType": "poi", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "off" }] }, { "featureType": "poi.park", "elementType": "geometry", "stylers": [{ "color": "#dedede" }, { "lightness": 21 }] }, { "featureType": "poi.park", "elementType": "labels.text.fill", "stylers": [{ "visibility": "off" }] }, { "featureType": "road", "elementType": "labels.text.stroke", "stylers": [{ "visibility": "off" }] }, { "featureType": "road.highway", "elementType": "geometry.fill", "stylers": [{ "color": "#ffffff" }, { "lightness": 17 }] }, { "featureType": "road.highway", "elementType": "geometry.stroke", "stylers": [{ "color": "#ffffff" }, { "lightness": 29 }, { "weight": 0.2 }] }, { "featureType": "road.arterial", "elementType": "geometry", "stylers": [{ "color": "#ffffff" }, { "lightness": 18 }] }, { "featureType": "road.arterial", "elementType": "labels.text.fill", "stylers": [{ "color": "#b1b1b1" }] }, { "featureType": "road.local", "elementType": "geometry", "stylers": [{ "color": "#ffffff" }, { "lightness": 16 }] }, { "featureType": "road.local", "elementType": "labels.text.fill", "stylers": [{ "color": "#cdcdcd" }] }, { "featureType": "transit", "elementType": "geometry", "stylers": [{ "color": "#f2f2f2" }, { "lightness": 19 }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "color": "#e9e9e9" }, { "lightness": 17 }] }, { "featureType": "water", "elementType": "labels.text.fill", "stylers": [{ "visibility": "off" }] }
//      // ],

//      // styles: [{ 'featureType': 'administrative', 'elementType': 'labels.text.fill', 'stylers': [{ 'color': '#444444' }] }, { 'featureType': 'landscape', 'elementType': 'all', 'stylers': [{ 'color': '#f2f2f2' }] }, { 'featureType': 'poi', 'elementType': 'all', 'stylers': [{ 'visibility': 'off' }] }, { 'featureType': 'road', 'elementType': 'all', 'stylers': [{ 'saturation': -100 }, { 'lightness': 45 }] }, { 'featureType': 'road.highway', 'elementType': 'all', 'stylers': [{ 'visibility': 'simplified' }] }, { 'featureType': 'road.arterial', 'elementType': 'labels.icon', 'stylers': [{ 'visibility': 'off' }] }, { 'featureType': 'transit', 'elementType': 'all', 'stylers': [{ 'visibility': 'off' }] }, { 'featureType': 'water', 'elementType': 'all', 'stylers': [{ 'color': '#4f595d' }, { 'visibility': 'on' }] }]
//      // styles:[{"featureType":"all","elementType":"labels.text.fill","stylers":[{"saturation":36},{"color":"#000000"},{"lightness":40}]},{"featureType":"all","elementType":"labels.text.stroke","stylers":[{"visibility":"on"},{"color":"#000000"},{"lightness":16}]},{"featureType":"all","elementType":"labels.icon","stylers":[{"visibility":"off"}]},{"featureType":"administrative","elementType":"geometry.fill","stylers":[{"color":"#000000"},{"lightness":20}]},{"featureType":"administrative","elementType":"geometry.stroke","stylers":[{"color":"#000000"},{"lightness":17},{"weight":1.2}]},{"featureType":"landscape","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":20}]},{"featureType":"landscape.natural.landcover","elementType":"labels.text.fill","stylers":[{"color":"#ff0000"},{"visibility":"on"}]},{"featureType":"poi","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":21}]},{"featureType":"road","elementType":"geometry.fill","stylers":[{"visibility":"on"}]},{"featureType":"road","elementType":"geometry.stroke","stylers":[{"visibility":"on"}]},{"featureType":"road.highway","elementType":"geometry.fill","stylers":[{"color":"#000000"},{"lightness":17}]},{"featureType":"road.highway","elementType":"geometry.stroke","stylers":[{"color":"#000000"},{"lightness":29},{"weight":0.2}]},{"featureType":"road.arterial","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":18}]},{"featureType":"road.local","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":16}]},{"featureType":"transit","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":19}]},{"featureType":"transit","elementType":"geometry.fill","stylers":[{"visibility":"on"}]},{"featureType":"transit","elementType":"labels.text","stylers":[{"hue":"#ff0000"}]},{"featureType":"water","elementType":"geometry","stylers":[{"color":"#000000"},{"lightness":17}]}]

//   };

//   // Get the HTML DOM element that will contain your map 
//   // We are using a div with id="map" seen below in the <body>

//   var mapElement = document.getElementById('map');

//   // Create the Google Map using our element and options defined above
//   var map = new google.maps.Map(mapElement, mapOptions);

//   // Let's also add a marker while we're at it
//   var index = 0;
//   var marker;
//   var position;
//   var geocoder = new google.maps.Geocoder();
//   var markers = [];
//   var length = data.length;

//   function loop() {
//      for (i = 0; i < data.length; i++) {
//         geocodeAddress(data[i]);
//      };
//   }

//   function markerCluster() {
//      var markerCluster = new MarkerClusterer(map, markers, {
//         imagePath: 'images/m'
//      });
//      console.log('1');
//   }

//   function geocodeAddress(data) {
//      geocoder.geocode({ 'address': data[2] }, function (results, status) {
//         //alert(status);
//         if (status == google.maps.GeocoderStatus.OK) {
//            createMarker(results[0].geometry.location, data);
//            console.log(markers);
//         } else {
//            alert("some problem in geocode " + status);
//         }
//      });
//   }

//   function createMarker(pos, data) {
//      var $marker_url2 = data[3];
//      var icon = {
//         url: $marker_url2,
//         scaledSize: new google.maps.Size(50, 50), // scaled size
//         origin: new google.maps.Point(0, 0), // origin
//         anchor: new google.maps.Point(25, 70) // anchor
//      }
//      marker = new MarkerWithLabel({
//         map: map,
//         position: pos,
//         animation: google.maps.Animation.DROP,
//         icon: icon,
//         labelContent: data[0],
//         labelClass: "labels-custom" + data[4] // the CSS class for the label
//      });
//      var contentString =
//         "<div class='info'><div class='info__header'><div style='background-image: url(" + data[1] + ")' class='header__avatar'></div></div><div class='info__footer'><h1 class='name__main'>" + data[0] + "</h1><div class='info__footer__content'><div class='footer__content__add'>" + data[2] + "</div><div class='footer__content__web'>https://vnpay.vn</div> <div class='footer__content__sdt'>0984888877</div><div class='button__wrap' onclick='routeOpen()'>Chỉ đường</div>";
//      var infowindow = new SnazzyInfoWindow({
//         marker: marker,
//         content: '<div id="iw_content">' + contentString + '</div>',
//         maxWidth: 500,
//         closeWhenOthersOpen: true
//      });
//      markers.push(marker);
//      if ((markers.length) == length) {
//         markerCluster();
//         console.log(2);
//         console.log(3);
//      }
//   }
//   loop();

//   if (markers.length > 0) {
//      $('#input').val('yes');
//   } else {

//      $('#input').val('no');
//   }


//   // marker.addListener('mouseout', function() {
//   //     infowindow.close(map, marker);
//   // });

//   // var tableid = 420419;
//   // var layer = new google.maps.FusionTablesLayer({
//   //     query: {
//   //         select: "kml_4326", 
//   //   from: tableid,
//   //   where: "name_0 IN ('Vietnam')"
//   //     },
//   //     styles: [{
//   //         polygonOptions: {
//   //             fillColor: '#cccccc',
//   //             fillOpacity: 0.05,
//   //         }
//   //     }]
//   // });
//}
$(document).ready(function () {
   //scroll-spy
   // Cache selectors
   var lastId,
      topMenu = $(".menu"),
      topMenuHeight = topMenu.outerHeight() + 15,
      // All list items
      menuItems = topMenu.find("a"),
      // Anchors corresponding to menu items
      scrollItems = menuItems.map(function () {
          var item = $(this).attr("href").charAt(0) !== "#" ? '' : $($(this).attr("href"));
         if (item.length) { return item; }
      });

   // Bind click handler to menu items
   // so we can get a fancy scroll animation
   menuItems.click(function (e) {
      $('#menu-trigger').prop('checked',false);
      BNS.off();
      var href = $(this).attr("href"),
         offsetTop = href === "#" ? 0 : $(href).offset().top - topMenuHeight + 1;
      $('html, body').stop().animate({
         scrollTop: offsetTop
      }, 300);
      e.preventDefault();
   });

   // Bind to scroll
   $(window).scroll(function () {
      // Get container scroll position
      var fromTop = $(this).scrollTop() + topMenuHeight;

      // Get id of current scroll item
      var cur = scrollItems.map(function () {
         if ($(this).offset().top < fromTop)
            return this;
      });
      // Get the id of the current element
      cur = cur[cur.length - 1];
      var id = cur && cur.length ? cur[0].id : "";

      if (lastId !== id) {
         lastId = id;
         // Set/remove active class
         menuItems
            .removeClass("active").filter("[href='#" + id + "']").addClass("active");
      }
   });
   //end scrollspy 
   $('.dragscroll a').click(function (event) {
      if ($(this).closest('.dragscroll').hasClass('dragging')) {
         event.preventDefault();
         return false;
      }
   });
   $('.scrollspy').scrollSpy({ 'scrollOffset': 120 });
   $('.background-circle').width($('.circle').width() - 96);
   $('.background-circle').height($('.circle').width() - 96);
   $('.background-circle').css('border-radius', ($('.circle').width() - 96) / 2);
   if ($(window).width() <= 998) {
      $('.circle').css('bottom', $('.sc--intro').height() - $('.sc--intro .row:nth-of-type(1)').height() - 30);
   }
   console.log($('.circle').width());
   console.log($('.circle').width() - 90);
   var owl4 = $('#banner-slider');
   owl4.owlCarousel({
      margin: 0,
      nav: true,
      navContainer: '.slider-nav',
      navText: ['', ''],
      items: 1,
   });
   $('.owl-partner').owlCarousel({
      margin: 0,
      nav: true,
      autoplay: true,
      autoplayTimeout:5000,
      loop: true,
      responsive: {
         // breakpoint from 0 up
         0: {
            items: 2
         },
         // breakpoint from 480 up
         480: {
            items: 3
         },
         // breakpoint from 768 up
         768: {
            items: 5,
            slideBy: 2,
         }
      }
   });
   $('.owl-sale').owlCarousel({
      margin: 0,
      nav: true,
      responsive: {
         // breakpoint from 0 up
         0: {
            items: 2
         },
         // breakpoint from 480 up
         480: {
            items: 3
         },
         // breakpoint from 768 up
         768: {
            items: 3
         }
      }
   });
   window.addEventListener('resize', function (event) {
      $('.background-circle').width($('.circle').width() - 96);
      $('.background-circle').height($('.circle').width() - 96);
      $('.background-circle').css('border-radius', ($('.circle').width() - 96) / 2);
      menu();
      if ($(window).width() <= 998) {
         $('.circle').css('bottom', $('.sc--intro').height() - $('.sc--intro .row:nth-of-type(1)').height() - 30);
      } else {
         $('.circle').removeAttr('style');
      }
   });
   menu();
   if ($('#myModal1').hasClass('modal')) {
      var youtubeFunc = '';
      var outerDiv = $("myModal1");
      var youtubeIframe = $("#myModal1 iframe")[0].contentWindow;
      $('#myModal1').on('hidden.bs.modal', function (e) {
         youtubeFunc = 'pauseVideo';
         youtubeIframe.postMessage('{"event":"command","func":"' + youtubeFunc + '","args":""}', '*');
      });
      $('#myModal1').on('show.bs.modal', function (e) {
         youtubeFunc = 'playVideo';
         youtubeIframe.postMessage('{"event":"command","func":"' + youtubeFunc + '","args":""}', '*');
      });
   }
});
(function(name) {
    function BNS() {
        var settings = {
            prevScroll: 0,
            prevPosition: '',
            prevOverflow: '',
            prevClasses: '',
            on: false,
            classes: ''
        };

        var getPrev = function() {
            settings.prevScroll = (window.pageYOffset || document.documentElement.scrollTop) - (document.documentElement.clientTop || 0);
            settings.prevPosition = document.body.style.position;
            settings.prevOverflow = document.body.style.overflow;
            settings.prevClasses = document.body.className;
        };

        return {
            set: function(data) {
                settings.classes = data.classes;
            },
            isOn: function() {
                return settings.on;
            },
            on: function(additionalClasses) {
                if (typeof additionalClasses === 'undefined') additionalClasses = '';

                if (settings.on) return;
                settings.on = true;

                getPrev();

                document.body.className = document.body.className + ' ' + settings.classes + ' ' + additionalClasses;
                document.body.style.top = -settings.prevScroll + 'px';
                setTimeout(function() {
                    document.body.style.position = 'fixed';
                }, 0); // WebKit/Blink bugfix
            },
            off: function() {
                if (!settings.on) return;
                settings.on = false;
                document.body.className = settings.prevClasses;
                document.body.style.top = 0;
                document.body.style.position = '';
                document.body.style.overflow = settings.prevOverflow;
                window.scrollTo(0, settings.prevScroll);
            }
        };
    }

    window[name] = new BNS();
})('BNS');
function menu() {
   
   var closedCSS = { 'max-height': 0, 'overflow': 'hidden' };
   if ((deviceIsMobile) || ($(window).width() <= menuSettings.breakpoint)) {
      if (menuSettings.iscrollMenuEnable) {
         var myScroll = new IScroll('.menu__content', {
            mouseWheel: true,
            scrollbars: false,
            disableMouse: false,
            click: false,
            preventDefaultException: { className: /(^|\s)menu__item__block(\s|$)/ } // add Class here to enable click
         });
         new ResizeSensor(document.querySelector('.menu__content'), function () {
            myScroll.refresh();
         });
         new ResizeSensor(document.querySelector('.menu__content__wrap'), function () {
            myScroll.refresh();
         });
      }
      $("#menu-trigger").on('change', function () {
         if ((this.checked) && (menuSettings.iscrollMenuEnable == false)) {
            BNS.on();
            $(".menu__content").scrollTop(0);
         } else {
            BNS.off();
            $('[data-accordion]').removeClass('open');
            $('[data-content]').css(closedCSS);
         }
      });
      $('.only-one [data-accordion]').accordion();
   };
   if ($(window).width() > menuSettings.breakpoint) {
      $('#menu-trigger').prop('checked', false);
      BNS.off();
      $('[data-accordion]').removeClass('open');
      $('[data-content]').removeAttr("style");
   };
};

