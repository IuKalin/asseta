/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['"Plus Jakarta Sans"', 'Inter', 'system-ui', '-apple-system', 'BlinkMacSystemFont', 'sans-serif'],
      },
      colors: {
        rausch: {
          50: '#FFF1F3',
          100: '#FFE4E8',
          200: '#FECDD6',
          500: '#FF385C', // Signature Airbnb Rausch
          600: '#E00B41',
          700: '#C13515',
        },
        airbnb: {
          black: '#222222',
          subtext: '#717171',
          border: '#EBEBEB',
          divider: '#DDDDDD',
          lightBg: '#F7F7F7',
          card: '#FFFFFF',
          green: '#008A05',
        },
        brand: {
          50: '#FFF1F3',
          100: '#FFE4E8',
          500: '#FF385C',
          600: '#E00B41',
          900: '#991B1B',
        },
        vault: {
          dark: '#F7F7F7',
          card: '#FFFFFF',
          border: '#EBEBEB'
        }
      }
    },
  },
  plugins: [],
}
