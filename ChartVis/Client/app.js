let currentChart = null;

$(document).ready(function () {
    // Modal ilk açılışta gösteriliyor
    $("#confirmSelection").click(function () {
        const requestData = {
            Host: $("#dbHost").val(),
            Port: $("#dbPort").val(),
            Username: $("#dbUser").val(),
            Password: $("#dbPass").val(),
            Database: $("#dbName").val(),
            SelectedObject: $("#dbObject").val()
        };

        const selectedChartType = $("#chartTypeSelect").val() || "bar";

        $("#popup").hide(); // Modal'ı kapat

        // API'ye istek gönder
        fetch("http://localhost:5193/api/Chart/getData", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(requestData)
        })
            .then(res => {
                if (!res.ok) throw new Error("Sunucudan hata döndü");
                return res.json();
            })
            .then(data => generateChart(data, selectedChartType))
            .catch(err => {
                console.error(err);
                alert("Veri alınırken hata oluştu: " + err.message);
            });
    });

    // Grafiği yeniden oluştur (modal'ı geri getir)
    $("#rebuildChart").click(function () {
        if (currentChart) {
            currentChart.destroy(); // önceki grafiği sil
            currentChart = null;
        }

        $("#popup").show(); // modal'ı tekrar göster
    });
});

// Grafik oluşturucu
function generateChart(data, chartType) {
    const ctx = document.getElementById("myChart").getContext("2d");

    if (currentChart) {
        currentChart.destroy();
    }

    currentChart = new Chart(ctx, {
        type: chartType,
        data: {
            labels: data.labels,
            datasets: [{
                label: "Grafik Verisi",
                data: data.values,
                backgroundColor: "rgba(75, 192, 192, 0.2)",
                borderColor: "rgba(75, 192, 192, 1)",
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                title: { display: true, text: 'Veri Görselleştirme Grafiği' }
            }
        }
    });
}
