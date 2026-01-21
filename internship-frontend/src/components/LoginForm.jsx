import { useId, useState } from "react";
import PropTypes from "prop-types";
import { login } from "../api";

export default function LoginForm({ onLoginSuccess, onSwitchToRegister }) {
  const usernameId = useId();
  const passwordId = useId();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      const data = await login(username, password);
      onLoginSuccess?.(data);
    } catch (err) {
      setError(err?.message || "Dogodila se greška prilikom prijave.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="form-card form-card--narrow">
      <h2>Prijava</h2>

      {error && <p style={{ color: "red", marginBottom: "10px" }}>{error}</p>}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "18px", fontSize: "1.08rem" }}>
          <label htmlFor={usernameId}>Korisničko ime:</label>
          <input
            id={usernameId}
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            style={{
              width: "100%",
              padding: "14px 14px",
              marginTop: "8px",
              fontSize: "1.05rem",
            }}
            required
          />
        </div>

        <div style={{ marginBottom: "18px", fontSize: "1.08rem" }}>
          <label htmlFor={passwordId}>Lozinka:</label>
          <input
            id={passwordId}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            style={{
              width: "100%",
              padding: "14px 14px",
              marginTop: "8px",
              fontSize: "1.05rem",
            }}
            required
          />
        </div>

        <button
          type="submit"
          disabled={loading}
          style={{
            width: "100%",
            padding: "14px 18px",
            marginTop: "18px",
            fontSize: "1.08rem",
            cursor: loading ? "not-allowed" : "pointer",
          }}
        >
          {loading ? "Prijava..." : "Prijavi se"}
        </button>

        <div
          style={{
            marginTop: "12px",
            textAlign: "center",
            fontSize: "0.95rem",
          }}
        >
          <span>Nemaš račun? </span>
          <button
            type="button"
            onClick={() => onSwitchToRegister?.()}
            style={{
              background: "none",
              border: "none",
              color: "#0d6efd",
              cursor: "pointer",
              padding: 0,
            }}
          >
            Registriraj se
          </button>
        </div>
      </form>
    </div>
  );
}

LoginForm.propTypes = {
  onLoginSuccess: PropTypes.func,
  onSwitchToRegister: PropTypes.func,
};
