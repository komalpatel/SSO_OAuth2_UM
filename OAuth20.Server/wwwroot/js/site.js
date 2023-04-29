// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var jq = (function () {


	function addBindings() {
		//window.onbeforeunload = function (e) {
		//	if (!window.isValidExit && location.href.indexOf("beegyweb") == -1) {
		//		return 'Wollen Sie die Seite wirklich verlassen?';
		//	}
		//}

		//fileUpload
		$('body').on('click', '#imageContainer', function () {
			document.getElementById('fileUpload').click();
		})


		$('#fileUpload').change(function () {
			if ($(this).val() != '') {
				var files = this.files;
				var length = files.length;
				for (var i = 0; i < length; i++) {
					setFile(files[i]);
				}
				$(this).val('');
			}
		})

		$('body').on('drop', '#sheet-1', function (e) {
			if (e.originalEvent.dataTransfer) {
				if (e.originalEvent.dataTransfer.files.length) {
					e.preventDefault();
					e.stopPropagation();
					var files = e.originalEvent.dataTransfer.files;
					var length = files.length;
					for (var i = 0; i < length; i++) {
						setFile(files[i]);
					}
				}
			}
		});

		function setFile(file) {
			var fileName = file.name.split('.')[0];
			var fileType = file.type;
			var reader = new FileReader();
			reader.id = new Date() * 1;
			reader.addEventListener('loadend', function (e) {
				if (file.type.indexOf('image') > -1) {
					setInputImageData(this.result);
				}
			});
			reader.readAsDataURL(file);
		}
		function setInputImageData(bin) {
			var img = new Image();
			img.onload = function () {
				var imgbase64 = bin.split(',')[1];
				var canvas = document.createElement('canvas');
				//var width = img.width > 1920 ? 1920 : img.width;
				//var height = img.width > 1920 ? 1920 / img.width * img.height : img.height;
				//var scale
				canvas.width = img.width;
				canvas.height = img.height;
				var ctx = canvas.getContext('2d');
				ctx.drawImage(img, 0, 0);
				var base64data = canvas.toDataURL("image/jpeg", 0.75);
				$('#addAssets').attr('src', base64data);
				$('#fileDataInput').attr('data-mimetype', "image/jpeg").val(base64data.split(',')[1]);
				$('#photoSvg').hide();
				$('#uploadImage').css('background-image', 'url('+base64data+')');

			}
			img.src = bin;
		}
		function setInputFileData(bin, fileType) {
			$('#fileDataInput').attr('data-mimetype', fileType).val(bin.split(',')[1]);
			angular.element($('.body-wrapper.ng-scope')).scope().setFileType(fileType);
		}
	}

	function guid() {
		function s4() {
			return Math.floor((1 + Math.random()) * 0x10000)
				.toString(16)
				.substring(1);
		}
		return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
			s4() + '-' + s4() + s4() + s4();
	}

	$(document).ready(function () {
		addBindings();
		$('body').on('click', '.fullview', function () {
			$('.col-paper').toggleClass('maximized');
			$g.updateDraggable();
		});
	})
}());