import { useId, useState } from "react";
import PropTypes from "prop-types";
import { register, login } from "../api";

export default function RegisterForm({ onRegisterSuccess, onSwitchToLogin }) {
  const usernameId = useId();
  const passwordId = useId();
  const confirmPasswordId = useId();

  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      // Ako je uspješna registracija ide auto login
      await register(username, password, confirmPassword);

      const loginResponse = await login(username, password);
      onRegisterSuccess?.(loginResponse);
    } catch (err) {
      setError(err?.message || "Dogodila se greška prilikom registracije.");
    } finally {
      setLoading(false);
    }
  }

  function handleSwitchToLoginClick() {
    onSwitchToLogin?.();
  }

  return (
    <div className="form-card form-card--narrow">
      <h2>Registracija</h2>

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

        <div style={{ marginBottom: "18px", fontSize: "1.08rem" }}>
          <label htmlFor={confirmPasswordId}>Potvrdi lozinku:</label>
          <input
            id={confirmPasswordId}
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
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
          {loading ? "Registracija..." : "Registriraj se"}
        </button>

        <div
          style={{
            marginTop: "12px",
            textAlign: "center",
            fontSize: "0.95rem",
          }}
        >
          <span>Već imaš račun? </span>
          <button
            type="button"
            onClick={handleSwitchToLoginClick}
            style={{
              background: "none",
              border: "none",
              color: "#0d6efd",
              cursor: "pointer",
              padding: 0,
            }}
          >
            Prijavi se
          </button>
        </div>
      </form>
    </div>
  );
}

RegisterForm.propTypes = {
  onRegisterSuccess: PropTypes.func,
  onSwitchToLogin: PropTypes.func,
};
