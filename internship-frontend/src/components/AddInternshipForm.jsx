import { useEffect, useId, useState } from "react";
import PropTypes from "prop-types";
import { getCategories, getCompanies, createInternship } from "../api";

export default function AddInternshipForm({ token, onCreated }) {
  const titleId = useId();
  const shortDescId = useId();
  const fullDescId = useId();
  const locationId = useId();
  const remoteId = useId();
  const featuredId = useId();
  const deadlineId = useId();
  const categorySelectId = useId();
  const companySelectId = useId();

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

      onCreated?.();
    } catch (err) {
      setError(err?.message || "Dogodila se greška.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="form-card">
      <h3>Dodaj novu praksu</h3>

      {error && <p style={{ color: "red" }}>{error}</p>}
      {success && <p style={{ color: "green" }}>{success}</p>}

      <form onSubmit={handleSubmit}>
        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={titleId}>Naslov:</label>
          <input
            id={titleId}
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            required
          />
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={shortDescId}>Kratki opis:</label>
          <input
            id={shortDescId}
            type="text"
            value={shortDescription}
            onChange={(e) => setShortDescription(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            required
          />
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={fullDescId}>Puni opis:</label>
          <textarea
            id={fullDescId}
            value={fullDescription}
            onChange={(e) => setFullDescription(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            rows={4}
          />
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={locationId}>Lokacija:</label>
          <input
            id={locationId}
            type="text"
            value={location}
            onChange={(e) => setLocation(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
          />
        </div>

        <div style={{ marginBottom: "8px", display: "flex", gap: "16px" }}>
          <div>
            <input
              id={remoteId}
              type="checkbox"
              checked={remote}
              onChange={(e) => setRemote(e.target.checked)}
              style={{ marginRight: "4px" }}
            />
            <label htmlFor={remoteId}>Remote</label>
          </div>

          <div>
            <input
              id={featuredId}
              type="checkbox"
              checked={isFeatured}
              onChange={(e) => setIsFeatured(e.target.checked)}
              style={{ marginRight: "4px" }}
            />
            <label htmlFor={featuredId}>Istaknuta praksa</label>
          </div>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={deadlineId}>Rok prijave:</label>
          <input
            id={deadlineId}
            type="date"
            value={deadline}
            onChange={(e) => setDeadline(e.target.value)}
            style={{ padding: "6px", marginLeft: "8px" }}
          />
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={categorySelectId}>Kategorija:</label>
          <select
            id={categorySelectId}
            value={categoryId}
            onChange={(e) => setCategoryId(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            required
          >
            <option value="">-- odaberi kategoriju --</option>
            {categories.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>

        <div style={{ marginBottom: "8px" }}>
          <label htmlFor={companySelectId}>Kompanija:</label>
          <select
            id={companySelectId}
            value={companyId}
            onChange={(e) => setCompanyId(e.target.value)}
            style={{ width: "100%", padding: "6px", marginTop: "4px" }}
            required
          >
            <option value="">-- odaberi kompaniju --</option>
            {companies.map((c) => (
              <option key={c.id} value={c.id}>
                {c.name}
              </option>
            ))}
          </select>
        </div>

        <button type="submit" disabled={loading}>
          {loading ? "Spremanje..." : "Dodaj praksu"}
        </button>
      </form>
    </div>
  );
}

AddInternshipForm.propTypes = {
  token: PropTypes.string,
  onCreated: PropTypes.func,
};
