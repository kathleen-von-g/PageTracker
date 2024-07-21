import pageTrackerLogo from '@/assets/images/book_logo_teal.svg'

function Header() {
  return (
    <div>
      <h1>PageTracker <img src={pageTrackerLogo} className="logo" alt="PageTracker logo" style={{ maxWidth: "1em" }} /></h1>
    </div>
  );
}

export default Header;