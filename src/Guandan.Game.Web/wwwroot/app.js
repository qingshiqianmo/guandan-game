const state = {
    game: null,
    selectedCardIds: new Set(),
};

const messageElement = document.getElementById("message");
const summaryElement = document.getElementById("summary");
const leadingPlayElement = document.getElementById("leading-play");
const playersElement = document.getElementById("players");
const logsElement = document.getElementById("logs");

document.getElementById("start-button").addEventListener("click", startGame);
document.getElementById("restart-button").addEventListener("click", restartGame);
document.getElementById("play-button").addEventListener("click", playCards);
document.getElementById("pass-button").addEventListener("click", passTurn);

loadState();

async function loadState() {
    const response = await fetch("/api/game/state");

    if (!response.ok) {
        renderGame(null);
        return;
    }

    state.game = await response.json();
    state.selectedCardIds.clear();
    renderGame(state.game);
}

async function startGame() {
    const response = await fetch("/api/game/start", { method: "POST" });
    state.game = await response.json();
    state.selectedCardIds.clear();
    setMessage("牌局已开始");
    renderGame(state.game);
}

async function restartGame() {
    const response = await fetch("/api/game/restart", { method: "POST" });
    state.game = await response.json();
    state.selectedCardIds.clear();
    setMessage("已重新发牌");
    renderGame(state.game);
}

async function playCards() {
    if (!state.game) {
        return;
    }

    const response = await fetch("/api/game/play", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            seat: state.game.currentTurn,
            cardIds: Array.from(state.selectedCardIds),
        }),
    });

    const result = await response.json();
    handleActionResult(result);
}

async function passTurn() {
    if (!state.game) {
        return;
    }

    const response = await fetch("/api/game/pass", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ seat: state.game.currentTurn }),
    });

    const result = await response.json();
    handleActionResult(result);
}

function handleActionResult(result) {
    if (!result.isSuccess) {
        setMessage(`操作失败：${result.errorCode}`);
        renderGame(result.game);
        return;
    }

    state.game = result.game;
    state.selectedCardIds.clear();
    setMessage("操作成功");
    renderGame(state.game);
}

function renderGame(game) {
    if (!game) {
        summaryElement.textContent = "当前还没有开始牌局。";
        leadingPlayElement.textContent = "";
        playersElement.innerHTML = "";
        logsElement.innerHTML = "";
        return;
    }

    summaryElement.textContent = `状态：${game.status} ｜ 当前回合：${game.currentTurn}号位 ｜ 级牌：${game.levelRank}`;
    leadingPlayElement.textContent = game.leadingPlay
        ? `桌面主牌：${game.leadingPlay.seat}号位 ${game.leadingPlay.cards.map(item => item.text).join(" ")}`
        : "桌面主牌：无";

    renderPlayers(game.players, game.currentTurn);
    renderLogs(game.logs);

    if (game.settlement) {
        const finishOrder = game.settlement.finishOrder.join("，");
        setMessage(`牌局结束，胜方：${game.settlement.winningTeam.join("、")}号位，顺序：${finishOrder}`);
    }
}

function renderPlayers(players, currentTurn) {
    playersElement.innerHTML = "";

    players.forEach(player => {
        const card = document.createElement("article");
        card.className = `player-card${player.seat === currentTurn ? " current" : ""}`;
        card.innerHTML = `<h2>${player.seat}号位 ${player.isFinished ? "（已出完）" : ""}</h2><p>手牌数：${player.cardCount}</p>`;

        const cardsElement = document.createElement("div");
        cardsElement.className = "cards";

        player.cards.forEach(item => {
            const chip = document.createElement("button");
            chip.type = "button";
            chip.className = `card-chip${state.selectedCardIds.has(item.id) ? " selected" : ""}`;
            chip.textContent = item.text;
            chip.disabled = player.seat !== currentTurn;
            chip.addEventListener("click", () => toggleCard(item.id));
            cardsElement.appendChild(chip);
        });

        card.appendChild(cardsElement);
        playersElement.appendChild(card);
    });
}

function renderLogs(logs) {
    logsElement.innerHTML = "";
    logs.slice().reverse().forEach(log => {
        const item = document.createElement("li");
        item.textContent = log;
        logsElement.appendChild(item);
    });
}

function toggleCard(cardId) {
    if (state.selectedCardIds.has(cardId)) {
        state.selectedCardIds.delete(cardId);
    } else {
        state.selectedCardIds.add(cardId);
    }

    renderGame(state.game);
}

function setMessage(message) {
    messageElement.textContent = message;
}
