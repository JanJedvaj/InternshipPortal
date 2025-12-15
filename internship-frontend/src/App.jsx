
import { useState } from "react";
import LoginForm from "./components/LoginForm";
import InternshipList from "./components/InternshipList";
import AddInternshipForm from "./components/AddInternshipForm";

function App() {
  const [auth, setAuth] = useState(() => {
    const stored = localStorage.getItem("auth");
    return stored ? JSON.parse(stored) : null;
  });

  const [reloadId, setReloadId] = useState(0); 

  function handleLoginSuccess(data) {
    const authData = {
      token: data.token,
      username: data.username,
      role: data.role,
      expiresAtUtc: data.expiresAtUtc,
    };

    setAuth(authData);
    localStorage.setItem("auth", JSON.stringify(authData));
  }

  function handleLogout() {
    setAuth(null);
    localStorage.removeItem("auth");
  }

  function handleInternshipCreated() {
    setReloadId(prev => prev + 1);
  }

  const isLoggedIn = !!auth;

  return (
    <div
      style={{
        fontFamily:
          "system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif",
      }}
    >
      <header
        style={{
          background: "#0d6efd",
          color: "white",
          padding: "16px",
          marginBottom: "24px",
        }}
      >
        <h1 style={{ margin: 0, textAlign: "center" }}>Internship Portal</h1>
      </header>

      <main
        style={
          !isLoggedIn
            ? {
                minHeight: "calc(100vh - 80px)",
                display: "flex",
                alignItems: "center",
                justifyContent: "center",
                padding: "0 16px",
              }
            : { padding: "0 16px 32px" }
        }
      >
        {!isLoggedIn ? (
          <LoginForm onLoginSuccess={handleLoginSuccess} />
        ) : (
          <div style={{ maxWidth: "900px", margin: "0 auto" }}>
            <div
              style={{
                display: "flex",
                justifyContent: "space-between",
                alignItems: "center",
                marginBottom: "20px",
              }}
            >
              <div>
                Prijavljen kao <strong>{auth.username}</strong> ({auth.role})
              </div>
              <button onClick={handleLogout}>Odjava</button>
            </div>

            <AddInternshipForm
              token={auth.token}
              onCreated={handleInternshipCreated}
            />

            <h2 style={{ textAlign: "center", marginBottom: "16px" }}>
              Dostupne prakse
            </h2>

            <InternshipList key={reloadId} />
          </div>
        )}
      </main>
    </div>
  );
}

export default App;
