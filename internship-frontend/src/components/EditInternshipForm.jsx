import { useEffect, useId, useState } from "react";
import PropTypes from "prop-types";

export default function EditInternshipForm({ internship, onCancel, onSave }) {
  const titleId = useId();
  const shortDescId = useId();
  const locationId = useId();
  const remoteId = useId();

  const [form, setForm] = useState(internship);

  useEffect(() => {
    setForm(internship);
  }, [internship]);

  if (!form) return null;

  function updateField(name, value) {
    setForm((prev) => ({ ...prev, [name]: value }));
  }

  function handleSubmit(e) {
    e.preventDefault();
    onSave(form);
  }

  return (
    <form
      onSubmit={handleSubmit}
      className="form-card"
      style={{ marginTop: 12 }}
    >
      <h3>Uredi praksu</h3>

      <div style={{ marginBottom: 8 }}>
        <label htmlFor={titleId}>Naslov</label>
        <input
          id={titleId}
          value={form.title || ""}
          onChange={(e) => updateField("title", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8 }}>
        <label htmlFor={shortDescId}>Kratki opis</label>
        <input
          id={shortDescId}
          value={form.shortDescription || ""}
          onChange={(e) => updateField("shortDescription", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8 }}>
        <label htmlFor={locationId}>Lokacija</label>
        <input
          id={locationId}
          value={form.location || ""}
          onChange={(e) => updateField("location", e.target.value)}
          style={{ width: "100%", padding: 8 }}
        />
      </div>

      <div style={{ marginBottom: 8, display: "flex", alignItems: "center" }}>
        <input
          id={remoteId}
          type="checkbox"
          checked={!!form.remote}
          onChange={(e) => updateField("remote", e.target.checked)}
          style={{ marginRight: 4 }}
        />
        <label htmlFor={remoteId}>Remote</label>
      </div>

      <div style={{ display: "flex", gap: 8 }}>
        <button type="submit">Spremi</button>
        <button type="button" onClick={onCancel}>
          Odustani
        </button>
      </div>
    </form>
  );
}

EditInternshipForm.propTypes = {
  internship: PropTypes.object,
  onCancel: PropTypes.func.isRequired,
  onSave: PropTypes.func.isRequired,
};
