import React from 'react';
import { Mail, Phone } from 'lucide-react';

export const Footer: React.FC = () => {
  return (
    <footer className="bg-[#F7F7F7] border-t border-[#EBEBEB] text-[#717171] text-xs transition-colors">
      <div className="max-w-7xl mx-auto px-6 py-12 lg:py-14">
        {/* 4 Columns Grid */}
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8 lg:gap-10">
          
          {/* Column 1: Company */}
          <div>
            <h4 className="text-sm font-bold text-[#222222] tracking-tight">Company</h4>
            <div className="w-8 h-0.5 bg-[#FF385C] mt-2 mb-4 rounded-full" />
            <ul className="space-y-2.5">
              <li>
                <a href="#about" className="hover:text-[#222222] hover:underline transition">About Us</a>
              </li>
              <li>
                <a href="#services" className="hover:text-[#222222] hover:underline transition">Our Services</a>
              </li>
              <li>
                <a href="#privacy" className="hover:text-[#222222] hover:underline transition">Privacy Policy</a>
              </li>
              <li>
                <a href="#affiliate" className="hover:text-[#222222] hover:underline transition">Affiliate Program</a>
              </li>
            </ul>
          </div>

          {/* Column 2: Get Help */}
          <div>
            <h4 className="text-sm font-bold text-[#222222] tracking-tight">Get Help</h4>
            <div className="w-8 h-0.5 bg-[#FF385C] mt-2 mb-4 rounded-full" />
            <ul className="space-y-2.5">
              <li>
                <a href="#faq" className="hover:text-[#222222] hover:underline transition">FAQ</a>
              </li>
              <li>
                <a href="#shipping" className="hover:text-[#222222] hover:underline transition">Shipping / Delivery</a>
              </li>
              <li>
                <a href="#audits" className="hover:text-[#222222] hover:underline transition">Security Audits</a>
              </li>
              <li>
                <a href="#status" className="hover:text-[#222222] hover:underline transition">Order Status</a>
              </li>
              <li>
                <a href="#payment" className="hover:text-[#222222] hover:underline transition">Payment Options</a>
              </li>
            </ul>
          </div>

          {/* Column 3: Online Services */}
          <div>
            <h4 className="text-sm font-bold text-[#222222] tracking-tight">Online Services</h4>
            <div className="w-8 h-0.5 bg-[#FF385C] mt-2 mb-4 rounded-full" />
            <ul className="space-y-2.5">
              <li>
                <a href="#vault" className="hover:text-[#222222] hover:underline transition">Continuity Vault</a>
              </li>
              <li>
                <a href="#handover" className="hover:text-[#222222] hover:underline transition">Emergency Handover</a>
              </li>
              <li>
                <a href="#storage" className="hover:text-[#222222] hover:underline transition">Backup & Storage</a>
              </li>
              <li>
                <a href="#advisor" className="hover:text-[#222222] hover:underline transition">Advisor Network</a>
              </li>
            </ul>
          </div>

          {/* Column 4: Follow Us & Contact */}
          <div>
            <h4 className="text-sm font-bold text-[#222222] tracking-tight">Follow Us</h4>
            <div className="w-8 h-0.5 bg-[#FF385C] mt-2 mb-4 rounded-full" />
            
            {/* Social Icons */}
            <div className="flex items-center gap-2.5 mb-5">
              {/* Facebook */}
              <a
                href="#facebook"
                aria-label="Facebook"
                className="w-8 h-8 rounded-full bg-white border border-[#DDDDDD] flex items-center justify-center text-[#222222] hover:bg-[#FF385C] hover:text-white hover:border-[#FF385C] transition shadow-xs"
              >
                <svg className="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                  <path d="M24 12.073c0-6.627-5.373-12-12-12s-12 5.373-12 12c0 5.99 4.388 10.954 10.125 11.854v-8.385H7.078v-3.47h3.047V9.43c0-3.007 1.792-4.669 4.533-4.669 1.312 0 2.686.235 2.686.235v2.953H15.83c-1.491 0-1.956.925-1.956 1.874v2.25h3.328l-.532 3.47h-2.796v8.385C19.612 23.027 24 18.062 24 12.073z" />
                </svg>
              </a>

              {/* Twitter / X */}
              <a
                href="#twitter"
                aria-label="Twitter"
                className="w-8 h-8 rounded-full bg-white border border-[#DDDDDD] flex items-center justify-center text-[#222222] hover:bg-[#FF385C] hover:text-white hover:border-[#FF385C] transition shadow-xs"
              >
                <svg className="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                  <path d="M18.244 2.25h3.308l-7.227 8.26 8.502 11.24H16.17l-5.214-6.817L4.99 21.75H1.68l7.73-8.835L1.254 2.25H8.08l4.713 6.231zm-1.161 17.52h1.833L7.084 4.126H5.117z" />
                </svg>
              </a>

              {/* Instagram */}
              <a
                href="#instagram"
                aria-label="Instagram"
                className="w-8 h-8 rounded-full bg-white border border-[#DDDDDD] flex items-center justify-center text-[#222222] hover:bg-[#FF385C] hover:text-white hover:border-[#FF385C] transition shadow-xs"
              >
                <svg className="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                  <path d="M12 2.163c3.204 0 3.584.012 4.85.07 3.252.148 4.771 1.691 4.919 4.919.058 1.265.069 1.645.069 4.849 0 3.205-.012 3.584-.069 4.849-.149 3.225-1.664 4.771-4.919 4.919-1.266.058-1.644.07-4.85.07-3.204 0-3.584-.012-4.849-.07-3.26-.149-4.771-1.699-4.919-4.92-.058-1.265-.07-1.644-.07-4.849 0-3.204.013-3.583.07-4.849.149-3.227 1.664-4.771 4.919-4.919 1.266-.057 1.645-.069 4.849-.069zm0-2.163c-3.259 0-3.667.014-4.947.072-4.358.2-6.78 2.618-6.98 6.98-.059 1.281-.073 1.689-.073 4.948 0 3.259.014 3.668.072 4.948.2 4.358 2.618 6.78 6.98 6.98 1.281.058 1.689.072 4.948.072 3.259 0 3.668-.014 4.948-.072 4.354-.2 6.782-2.618 6.979-6.98.059-1.28.073-1.689.073-4.948 0-3.259-.014-3.667-.072-4.947-.196-4.354-2.617-6.78-6.979-6.98-1.281-.059-1.69-.073-4.949-.073zm0 5.838c-3.403 0-6.162 2.759-6.162 6.162s2.759 6.163 6.162 6.163 6.162-2.759 6.162-6.163c0-3.403-2.759-6.162-6.162-6.162zm0 10.162c-2.209 0-4-1.79-4-4 0-2.209 1.791-4 4-4s4 1.791 4 4c0 2.21-1.791 4-4 4zm6.406-11.845c-.796 0-1.441.645-1.441 1.44s.645 1.44 1.441 1.44c.795 0 1.439-.645 1.439-1.44s-.644-1.44-1.439-1.44z" />
                </svg>
              </a>

              {/* LinkedIn */}
              <a
                href="#linkedin"
                aria-label="LinkedIn"
                className="w-8 h-8 rounded-full bg-white border border-[#DDDDDD] flex items-center justify-center text-[#222222] hover:bg-[#FF385C] hover:text-white hover:border-[#FF385C] transition shadow-xs"
              >
                <svg className="w-3.5 h-3.5 fill-current" viewBox="0 0 24 24">
                  <path d="M19 0h-14c-2.761 0-5 2.239-5 5v14c0 2.761 2.239 5 5 5h14c2.762 0 5-2.239 5-5v-14c0-2.761-2.238-5-5-5zm-11 19h-3v-11h3v11zm-1.5-12.268c-.966 0-1.75-.79-1.75-1.764s.784-1.764 1.75-1.764 1.75.79 1.75 1.764-.783 1.764-1.75 1.764zm13.5 12.268h-3v-5.604c0-3.368-4-3.113-4 0v5.604h-3v-11h3v1.765c1.396-2.586 7-2.777 7 2.476v6.759z" />
                </svg>
              </a>
            </div>

            {/* Email */}
            <div className="space-y-1.5 mb-2">
              <span className="text-[#222222] font-semibold block">Email:</span>
              <a
                href="mailto:globalhelcurt14092005@gmail.com"
                className="flex items-center gap-1.5 text-[#717171] hover:text-[#222222] hover:underline transition font-mono break-all"
              >
                <Mail className="w-3.5 h-3.5 shrink-0 text-[#FF385C]" />
                <span>globalhelcurt14092005@gmail.com</span>
              </a>
            </div>

            {/* Hotline */}
            <div className="space-y-1">
              <span className="text-[#222222] font-semibold flex items-center gap-1.5">
                <Phone className="w-3.5 h-3.5 text-[#FF385C]" />
                Hotline 24/7: <span className="font-bold text-[#FF385C]">1900 8899</span>
              </span>
              <span className="text-[11px] text-[#717171] block">
                (Hỗ trợ bàn giao khẩn cấp)
              </span>
            </div>
          </div>

        </div>

        {/* Bottom Sub-Bar */}
        <div className="mt-10 pt-6 border-t border-[#EBEBEB] flex flex-col sm:flex-row items-center justify-between gap-3 text-[11px] text-[#717171]">
          <div>
            Copyright © 2025 <strong className="text-[#222222] font-semibold">Asseta Monorepo</strong>. All rights reserved.
          </div>
          <div className="font-mono text-[#717171]">
            Version v1.0.0-PROD (Build: 2025.10.14-REV5)
          </div>
        </div>
      </div>
    </footer>
  );
};
