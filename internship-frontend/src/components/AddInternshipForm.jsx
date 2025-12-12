
import { useEffect, useState } from "react";
import { getCategories, getCompanies, createInternship } from "../api";

export default function AddInternshipForm({ token, onCreated }) {
  const [title, setTitle] = useState("");
  const [shortDescription, setShortDescription] = useState("");
  const [fullDescription, setFullDescription] = useState("");
  const [location, setLocation] = useState("");
  const [remote, setRemote] = useState(false);
  const [isFeatured, setIsFeatured] = useState(false);
  const [deadline, setDeadline] = useState("");
  const [categoryId, setCategoryId] = useState("");
  const [companyId, setCompanyId] = useState("");

  const [categories, setCategories] = useState([]);
  const [companies, setCompanies] = useState([]);

  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    getCategories()
      .then(setCategories)
      .catch(() => setError("Ne mogu dohvatiti kategorije."));

    getCompanies()
      .then(setCompanies)
      .catch(() => setError("Ne mogu dohvatiti kompanije."));
  }, []);

  async function handleSubmit(e) {
    e.preventDefault();
    setError("");
    setSuccess("");

    if (!categoryId || !companyId) {
      setError("Odaberi kategoriju i kompaniju.");
      return;
    }

    const internship = {
      title,
      shortDescription,
      fullDescription,
      location,
      remote,
      isFeatured,
      categoryId: Number(categoryId),
      companyId: Number(companyId),
      postedAt: new Date().toISOString(),
      deadline: deadline ? new Date(deadline).toISOString() : null,
    };

    setLoading(true);
    try {
      await createInternship(internship, token);
      setSuccess("Praksa je uspješno dodana.");
      setTitle("");
      setShortDescription("");
      setFullDescription("");
      setLocation("");
      setRemote(false);
      setIsFeatured(false);
      setDeadline("");
      setCategoryId("");
      setCompanyId("");

      if (onCreated) {
        onCreated();
      }
    } catch (err) {
      setError(err.message || "Dogodila se greška.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div
      style={{
        border: "1px solid #ddd",
        borderRadius: "8px",
        padding: "16px",
        marginBottom: "24px",
      }}
    >
      <h3>Dodaj novu praksu</h3>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {success && <p style={{ color: "green" }}>{success}</p>}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "8px" }}>
          <label>
            Naslov:
            <input
              type="text"
              value={title}
              onChange={e => setTitle(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
              required
            />
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Kratki opis:
            <input
              type="text"
              value={shortDescription}
              onChange={e => setShortDescription(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
              required
            />
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Puni opis:
            <textarea
              value={fullDescription}
              onChange={e => setFullDescription(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
              rows={4}
            />
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Lokacija:
            <input
              type="text"
              value={location}
              onChange={e => setLocation(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            />
          </label>
        </div>

        <div style={{ marginBottom: "8px", display: "flex", gap: "16px" }}>
          <label>
            <input
              type="checkbox"
              checked={remote}
              onChange={e => setRemote(e.target.checked)}
              style={{ marginRight: "4px" }}
            />
            Remote
          </label>

          <label>
            <input
              type="checkbox"
              checked={isFeatured}
              onChange={e => setIsFeatured(e.target.checked)}
              style={{ marginRight: "4px" }}
            />
            Istaknuta praksa
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Rok prijave:
            <input
              type="date"
              value={deadline}
              onChange={e => setDeadline(e.target.value)}
              style={{ padding: "6px", marginLeft: "8px" }}
            />
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Kategorija:
            <select
              value={categoryId}
              onChange={e => setCategoryId(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
              required
            >
              <option value="">-- odaberi kategoriju --</option>
              {categories.map(c => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label>
            Kompanija:
            <select
              value={companyId}
              onChange={e => setCompanyId(e.target.value)}
              style={{ width: "100%", padding: "6px", marginTop: "4px" }}
              required
            >
              <option value="">-- odaberi kompaniju --</option>
              {companies.map(c => (
                <option key={c.id} value={c.id}>
                  {c.name}
                </option>
              ))}
            </select>
          </label>
        </div>

        <button type="submit" disabled={loading}>
          {loading ? "Spremanje..." : "Dodaj praksu"}
        </button>
      </form>
    </div>
  );
}
