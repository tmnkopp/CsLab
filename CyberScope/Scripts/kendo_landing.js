$("#chart").kendoChart({ 
    seriesDefaults: {
        type: "column",
        labels: {
            visible: true,
            background: "transparent"
        }
    },
    series: [{
        data: mo_tot,
        color: "#a0b0c0"
    }, { 
        data: mo_ot,
        color: "#ffb0c0"
     }],
    valueAxis: { max: 28, majorGridLines: { visible: false }, visible: false },

    categoryAxis: { majorGridLines: { visible: false }, line: { visible: false } }

});