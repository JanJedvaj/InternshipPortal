
import { useEffect, useState } from "react";
import { getInternships } from "../api";

function formatDate(dateString) {
  if (!dateString) return "-";
  const date = new Date(dateString);
  return date.toLocaleDateString("hr-HR");
}

export default function InternshipList() {
  const [internships, setInternships] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    getInternships()
      .then(data => {
        setInternships(data);
        setLoading(false);
      })
      .catch(err => {
        setError(err.message || "Dogodila se greška.");
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Učitavanje...</p>;
  if (error) return <p style={{ color: "red" }}>Greška: {error}</p>;

  return (
    <div style={{ maxWidth: "900px", margin: "0 auto" }}>
      {internships.map(i => (
        <div
          key={i.id}
          style={{
            padding: "16px",
            border: "1px solid #ddd",
            marginBottom: "12px",
            borderRadius: "6px"
          }}
        >
          <h2>{i.title}</h2>
          <p>{i.shortDescription}</p>
          <p><strong>Lokacija:</strong> {i.location}</p>
          <p><strong>Remote:</strong> {i.remote ? "Da" : "Ne"}</p>
          <p><strong>Objavljeno:</strong> {formatDate(i.postedAt)}</p>

          {i.deadline && (
            <p><strong>Rok prijave:</strong> {formatDate(i.deadline)}</p>
          )}
        </div>
      ))}
    </div>
  );
}
