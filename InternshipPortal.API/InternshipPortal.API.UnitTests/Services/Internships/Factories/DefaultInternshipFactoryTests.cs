using System;
using Xunit;
using InternshipPortal.API.Data.EF;
using InternshipPortal.API.Exceptions;
using InternshipPortal.API.Services.Internships.Factories;

namespace InternshipPortal.API.UnitTests.Services.Internships.Factories
{
    public class DefaultInternshipFactoryTests
    {
        [Fact]
        public void CreateNew_Null_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            Assert.Throws<ValidationException>(() => sut.CreateNew(null!));
        }

        [Fact]
        public void CreateNew_MissingTitle_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.Title = "   ";

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_MissingShortDescription_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.ShortDescription = null!;

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_MissingFullDescription_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.FullDescription = "";

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_MissingLocation_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.Location = null!;

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_InvalidCompanyId_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.CompanyId = 0;

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_InvalidCategoryId_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.CategoryId = -5;

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_DeadlineInPast_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.Deadline = DateTime.UtcNow.Date.AddDays(-1);

            Assert.Throws<ValidationException>(() => sut.CreateNew(input));
        }

        [Fact]
        public void CreateNew_TrimsStrings_AndResetsId()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.Id = 123;
            input.Title = "  Title  ";
            input.ShortDescription = "  Short  ";
            input.FullDescription = "  Full  ";
            input.Location = "  Zagreb  ";

            var result = sut.CreateNew(input);

            Assert.NotSame(input, result);
            Assert.Equal(0, result.Id); // EF će dodijeliti
            Assert.Equal("Title", result.Title);
            Assert.Equal("Short", result.ShortDescription);
            Assert.Equal("Full", result.FullDescription);
            Assert.Equal("Zagreb", result.Location);
        }

        [Fact]
        public void CreateNew_WhenPostedAtDefault_SetsUtcNow()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            input.PostedAt = default;

            var before = DateTime.UtcNow;
            var result = sut.CreateNew(input);
            var after = DateTime.UtcNow;

            Assert.True(result.PostedAt >= before && result.PostedAt <= after);
        }

        [Fact]
        public void CreateNew_WhenPostedAtProvided_KeepsIt()
        {
            var sut = new DefaultInternshipFactory();

            var input = ValidInternship();
            var posted = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);
            input.PostedAt = posted;

            var result = sut.CreateNew(input);

            Assert.Equal(posted, result.PostedAt);
        }

        [Fact]
        public void ApplyUpdates_ExistingNull_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            Assert.Throws<ValidationException>(() => sut.ApplyUpdates(null!, ValidInternship()));
        }

        [Fact]
        public void ApplyUpdates_UpdatesNull_ThrowsValidationException()
        {
            var sut = new DefaultInternshipFactory();

            Assert.Throws<ValidationException>(() => sut.ApplyUpdates(ValidInternship(), null!));
        }

        [Fact]
        public void ApplyUpdates_TrimsAndUpdatesFields()
        {
            var sut = new DefaultInternshipFactory();

            var existing = ValidInternship();
            existing.Id = 5;

            var updates = ValidInternship();
            updates.Title = "  NewTitle  ";
            updates.ShortDescription = "  NewShort  ";
            updates.FullDescription = "  NewFull  ";
            updates.Location = "  Split  ";
            updates.IsFeatured = true;
            updates.Remote = true;
            updates.CompanyId = 99;
            updates.CategoryId = 77;
            updates.Deadline = DateTime.UtcNow.Date.AddDays(10);

            var result = sut.ApplyUpdates(existing, updates);

            Assert.Same(existing, result);
            Assert.Equal("NewTitle", existing.Title);
            Assert.Equal("NewShort", existing.ShortDescription);
            Assert.Equal("NewFull", existing.FullDescription);
            Assert.Equal("Split", existing.Location);
            Assert.True(existing.IsFeatured);
            Assert.True(existing.Remote);
            Assert.Equal(99, existing.CompanyId);
            Assert.Equal(77, existing.CategoryId);
            Assert.Equal(updates.Deadline, existing.Deadline);
        }

        [Fact]
        public void ApplyUpdates_WhenExistingPostedAtDefault_SetsFromUpdatesOrUtcNow()
        {
            var sut = new DefaultInternshipFactory();

            var existing = ValidInternship();
            existing.PostedAt = default;

            var updates = ValidInternship();
            updates.PostedAt = default;

            var before = DateTime.UtcNow;
            sut.ApplyUpdates(existing, updates);
            var after = DateTime.UtcNow;

            Assert.True(existing.PostedAt >= before && existing.PostedAt <= after);
        }

        [Fact]
        public void ApplyUpdates_DoesNotOverridePostedAt_WhenExistingAlreadySet()
        {
            var sut = new DefaultInternshipFactory();

            var existing = ValidInternship();
            existing.PostedAt = new DateTime(2024, 12, 12, 10, 0, 0, DateTimeKind.Utc);

            var updates = ValidInternship();
            updates.PostedAt = new DateTime(2025, 1, 1, 10, 0, 0, DateTimeKind.Utc);

            sut.ApplyUpdates(existing, updates);

            // Po tvojoj logici: PostedAt se postavlja samo ako je existing.PostedAt == default
            Assert.Equal(new DateTime(2024, 12, 12, 10, 0, 0, DateTimeKind.Utc), existing.PostedAt);
        }

        private static Internship ValidInternship()
        {
            return new Internship
            {
                Id = 0,
                Title = "Title",
                ShortDescription = "Short",
                FullDescription = "Full",
                IsFeatured = false,
                Remote = false,
                Location = "Zagreb",
                PostedAt = DateTime.UtcNow,
                Deadline = DateTime.UtcNow.Date.AddDays(7),
                CompanyId = 1,
                CategoryId = 1
            };
        }
    }
}
