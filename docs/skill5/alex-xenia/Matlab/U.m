%U function

function V = U(t)
    if t> 0 && t< 0.5E-9
         V = 1;
    else
         V = 0;
    end
end