var createSlider = function (id, idx, starts, ends, pvS, pvE) {
	var slider = document.getElementById(id);
	var tStart = new Array();
	var tConnect = [false];
	var hitFirst = false;
	if (starts != null) {

		if (starts != -1) {
			tStart.push(getTime(starts));
			hitFirst = true;
		}
		//if (mixedS != -1) {
		//	if (hitFirst) { tConnect.push(true); }
		//	tStart.push(getTime(mixedS));
		//	hitFirst = true;
		//}
		if (pvS != -1) {
			if (hitFirst) { tConnect.push(true); }
			tStart.push(getTime(pvS));
			hitFirst = true;
		}
		if (pvE != -1) {
			tConnect.push(true);
			tStart.push(getTime(pvE));
		}
		//if (mixedE != -1) {
		//	tConnect.push(true); 
		//	tStart.push(getTime(mixedE));
		//}
		if (ends != -1) {
			tConnect.push(true);
			tStart.push(getTime(ends));
		}
		tConnect.push(false);
	} else {
		tStart = [6, 9, 15, 18];
		tConnect = [false, true, true, true, false];
	}
	var currIdx = idx;
	noUiSlider.create(slider, {
		start: tStart,
		connect: tConnect,
		range: {
			'min': 0,
			'max': 24
		},
		tooltips: true,
		step: 0.25,
		// Show a scale with the slider
		pips: {
			mode: 'values',
			stepped: true,
			values: [0, 6, 12, 18, 24],
			density: 24
		},
		format: {
			// 'to' the formatted value. Receives a number.
			to: function (value) {
				var hour = parseInt(value);
				var minutes = Math.round((value - hour) * 4, 0) * 15;
				return ("0" + hour).slice(-2) + ':' + ("0" + minutes).slice(-2);
			},
			// 'from' the formatted value.
			// Receives a string, should return a number.
			from: function (value) {
				return Number(value.replace(',-', ''));
			}
		}
	});
	slider.noUiSlider.on('update', function (values) {
		var selectedValues = slider.noUiSlider.get(true);
		if (selectedValues.length == 2) {
			document.getElementById('Opens' + currIdx).value = checkTime(selectedValues[0]);
			document.getElementById('Closes' + currIdx).value = checkTime(selectedValues[1]);
		}
		if (selectedValues.length == 4) {
			document.getElementById('Opens' + currIdx).value = checkTime(selectedValues[0]);
			document.getElementById('PvStarts' + currIdx).value = checkTime(selectedValues[1]);
			document.getElementById('PvEnds' + currIdx).value = checkTime(selectedValues[2]);
			document.getElementById('Closes' + currIdx).value = checkTime(selectedValues[3]);
		}
	});

}
var checkTime = function (input) {
	return Math.round(input * 100) / 100;
}
var getTime = function (val) {
	return Math.round(val / 900) / 4;
}