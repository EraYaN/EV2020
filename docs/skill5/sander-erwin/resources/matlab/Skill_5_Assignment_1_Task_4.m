L0 = 0.5e-6;
C0 = 2e-12;
Zl = 50 + 50i;
Vmax = 1;
tw = 0.5e-9;
l = 1.2;

Z0 = sqrt(L0/C0);
Gamma = (Zl - Z0) / (Zl + Z0);
gamma = sqrt(C0 * L0);

t = 0:0.01e-9:5e-9;

V = zeros(length(t));
I = zeros(length(t));
real_power = zeros(length(t));
imag_power = zeros(length(t));
abs_power = zeros(length(t));
eta = zeros(length(t));

U = @(t) Vmax * (heaviside(t) - heaviside(t - tw));

for j = 1:length(t)
    V(j) =          U(t(j) - gamma * l) ...
        + Gamma *   U(t(j) + gamma * (l - 2 * l)) ...
        - Gamma *   U(t(j) - gamma * (l + 2 * l)) ...
        - Gamma.^2 *U(t(j) + gamma * (l - 4 * l)) ...
        + Gamma.^2 *U(t(j) - gamma * (l + 4 * l)) ...
        - Gamma.^3 *U(t(j) - gamma * (l + 6 * l)) ...
        + Gamma.^3 *U(t(j) + gamma * (l - 6 * l));
    I(j) = V(j)/Zl;
    
    real_power(j) = real(V(j).^2 / Zl);
    imag_power(j) = imag(V(j).^2 / Zl);
    abs_power(j) = abs(V(j).^2 / Zl);
    if abs_power(j) ~= 0
        eta(j) = real_power(j) / abs_power(j) * 100;
    else
        eta(j) = 0;
    end
end

plot(t, [real_power imag_power abs_power])
%plot(t, eta)