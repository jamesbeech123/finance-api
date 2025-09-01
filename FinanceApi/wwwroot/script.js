const apiBase = "/api"; 

// Fetch clients from API
async function fetchClients() {
    try {
        const res = await fetch(`${apiBase}/clients`);
        if (!res.ok) throw new Error("Failed to fetch clients");
        const clients = await res.json();
        displayClients(clients);
    } catch (err) {
        console.error(err);
        document.getElementById("clients").innerHTML = "<p>Error loading clients.</p>";
    }
}

// Display clients and portfolios
function displayClients(clients) {
    const container = document.getElementById("clients");
    container.innerHTML = "";

    clients.forEach(client => {
        const clientCard = document.createElement("div");
        clientCard.classList.add("card");
        clientCard.innerHTML = `<h2>${client.fullName} (Net Worth: $${client.netWorth.toLocaleString()})</h2>`;

        if (client.portfolios && client.portfolios.length > 0) {
            client.portfolios.forEach(p => {
                const portDiv = document.createElement("div");
                portDiv.classList.add("portfolio");
                portDiv.innerHTML = `<h3>${p.name} - Total Investment: $${p.totalInvestment.toLocaleString()}</h3>`;

                const invUl = document.createElement("ul");
                invUl.classList.add("investment-list");

                p.investments.forEach(inv => {
                    const invLi = document.createElement("li");
                    invLi.textContent = `${inv.assetName} (${inv.assetType}) — Units: ${inv.units.toLocaleString()}, Price: $${inv.currentPrice}`;
                    invUl.appendChild(invLi);
                });

                portDiv.appendChild(invUl);
                clientCard.appendChild(portDiv);
            });
        }

        container.appendChild(clientCard);

        // Fetch analytics for the first client only
        if (clients.indexOf(client) === 0) {
            fetchAnalytics(client.id);
        }
    });
}

// Fetch client analytics
async function fetchAnalytics(clientId) {
    try {
        const res = await fetch(`${apiBase}/Clients/${clientId}/analytics`);
        if (!res.ok) throw new Error("Failed to fetch analytics");
        const analytics = await res.json();
        displayChart(analytics.diversification);
    } catch (err) {
        console.error(err);
    }
}

// Display pie chart for diversification
function displayChart(diversification) {
    const canvasId = 'diversificationChart';
    let canvas = document.getElementById(canvasId);

    // Create canvas if it doesn't exist
    if (!canvas) {
        canvas = document.createElement("canvas");
        canvas.id = canvasId;
        document.querySelector(".container").appendChild(canvas);
    }

    const ctx = canvas.getContext('2d');

    new Chart(ctx, {
        type: 'pie',
        data: {
            labels: Object.keys(diversification),
            datasets: [{
                data: Object.values(diversification),
                backgroundColor: ['#FF6384', '#36A2EB', '#FFCE56', '#4BC0C0']
            }]
        },
        options: {
            responsive: true,
            plugins: { legend: { position: 'bottom' } }
        }
    });
}

// Initialize
fetchClients();
